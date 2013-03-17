using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using ThreadState = System.Threading.ThreadState;

namespace Dixie.Core
{
	public class Engine
	{
		public Engine(InitialGridState initialState, ILog baseLog)
		{
			this.initialState = initialState;
			this.baseLog = baseLog;
		}

		public ComparisonTestResult TestAlgorithms(IEnumerable<ISchedulerAlgorithm> algorithms, TimeSpan testDuration, TimeSpan intermediateCheckPeriod)
		{
			var result = new ComparisonTestResult();
			foreach (ISchedulerAlgorithm algorithm in algorithms)
			{
				AlgorithmNamesHelper.PrepareAlgorithmName(algorithm);
				result.AddAlgorithmResult(algorithm, TestAlgorithm(algorithm, testDuration, intermediateCheckPeriod));
			}
			return result;
		}

		public AlgorithmTestResult TestAlgorithm(ISchedulerAlgorithm algorithm, TimeSpan testDuration, TimeSpan intermediateCheckPeriod)
		{
			testLog = new PrefixedILogWrapper(baseLog, "Engine-" + algorithm.Name);
			LogTestAlgorithm(testDuration);

			topology = initialState.Topology.Clone();
			master = new Master(initialState.EngineSettings.DeadabilityThreshold, testLog);
			schedulerAlgorithm = algorithm;
			schedulerAlgorithm.Reset();
			garbageCollector = new GarbageCollector(initialState.EngineSettings.DeadabilityThreshold);
			topologyMutator = new CompositeMutator(initialState.RandomSeed, topology.WorkerNodesCount, 
				initialState.EngineSettings.RemoveNodesProbability, 
				initialState.EngineSettings.AddNodesProbability, 
				initialState.TopologySettings,
				garbageCollector
			);
			heartBeatProcessor = new HeartBeatProcessor(topology, master, initialState.EngineSettings.HeartBeatPeriod);
			tasksGenerator = new TasksGenerator(initialState, testLog);
			errorsCount = 0;
			LogTopologyBoundaries(topology.WorkerNodesCount);

			if (engineThreads == null)
				engineThreads = new Thread[5];
			var hbSyncEvent = new ManualResetEvent(false);
			var commonSyncEvent = new ManualResetEvent(false);
			engineThreads[0] = StartHeartBeatsMechanism(hbSyncEvent);
			engineThreads[1] = StartTopologyMutations(commonSyncEvent);
			engineThreads[2] = StartTaskGeneration(commonSyncEvent);
			engineThreads[3] = StartSchedulerAlgorithm(commonSyncEvent);
			engineThreads[4] = StartGarbageCollection(commonSyncEvent);
			var watch = new Stopwatch();
			var testResult = new AlgorithmTestResult();

			// (iloktionov): Включим механизм HBM и дождемся, пока все ноды пропингуют Мастера. 
			hbSyncEvent.Set();
			LogEnabledHBM();
			WaitForMasterStateWarmup();
			// (iloktionov): Теперь запустим остальные потоки и таймер теста.
			commonSyncEvent.Set();
			watch.Start();

			while (watch.Elapsed < testDuration)
			{
				TimeSpan timeBeforeEnd = testDuration - watch.Elapsed;
				if (timeBeforeEnd <= TimeSpan.Zero)
					break;
				Thread.Sleep(ExtendedMath.Min(intermediateCheckPeriod, timeBeforeEnd + TimeSpan.FromMilliseconds(1)));
				testResult.AddIntermediateResult(master.GetTotalWorkDone(), watch.Elapsed);
				if (onIntermediateResult != null)
					onIntermediateResult(testResult.IntermediateResults.Last(), algorithm.Name);
				CheckErrors();
			}

			// (iloktionov): Теперь остановим подсчет результатов и возьмем финальное значение перед завершением потоков.
			master.DisableAccumulatingResults();
			testResult.AddIntermediateResult(master.GetTotalWorkDone(), watch.Elapsed);
			StopThreads();
			LogResult(testResult);
			CheckErrors();
			return testResult;
		}

		public void Stop()
		{
			StopThreads();
		}

		public int GetRunningThreadsCount()
		{
			if (engineThreads == null)
				return 0;
			return engineThreads.Where(engineThread => engineThread != null).Count(engineThread => engineThread.ThreadState == ThreadState.Running);
		}

		public void SetOnIntermediateResultCallback(Action<IntermediateTestResult, string> callback)
		{
			onIntermediateResult = callback;
		}

		public Topology Topology
		{
			get { return topology; }
		}

		public InitialGridState InitialState
		{
			get { return initialState; }
		}

		private Thread StartHeartBeatsMechanism(WaitHandle syncEvent)
		{
			return ThreadRunner.RunPeriodicAction(heartBeatProcessor.DeliverMessagesAndResponses, TimeSpan.Zero, thread => thread.Priority = ThreadPriority.Highest, syncEvent, OnUnexpectedError);
		}

		private Thread StartTopologyMutations(WaitHandle syncEvent)
		{
			return ThreadRunner.RunPeriodicAction(() => topologyMutator.Mutate(topology), initialState.EngineSettings.TopologyMutatorRunPeriod, null, syncEvent, OnUnexpectedError);
		}

		private Thread StartTaskGeneration(WaitHandle syncEvent)
		{
			return ThreadRunner.RunPeriodicAction(() => master.RefillTasksIfNeeded(tasksGenerator), initialState.EngineSettings.TasksGeneratorRunPeriod, null, syncEvent, OnUnexpectedError);
		}

		private Thread StartSchedulerAlgorithm(WaitHandle syncEvent)
		{
			return ThreadRunner.RunPeriodicAction(() => master.ExecuteSchedulerAlgorithm(schedulerAlgorithm), initialState.EngineSettings.SchedulingAlgorithmRunPeriod, null, syncEvent, OnUnexpectedError);
		}

		private Thread StartGarbageCollection(WaitHandle syncEvent)
		{
			return ThreadRunner.RunPeriodicAction(() => garbageCollector.CollectGarbage(master), initialState.EngineSettings.GarbageCollectorRunPeriod, null, syncEvent, OnUnexpectedError);
		}

		private void WaitForMasterStateWarmup()
		{
			while (master.AliveNodesCount < topology.WorkerNodesCount)
				Thread.Sleep(10);
			LogMasterWarmedUp();
		}

		private void StopThreads()
		{
			if (engineThreads == null)
				return;
			LogStoppingThreads();
			ThreadRunner.StopThreads(engineThreads);
			LogStoppedThreads();
		}

		private void OnUnexpectedError(Exception error)
		{
			LogUnexpectedError(error);
			Interlocked.Increment(ref errorsCount);
		}

		private void CheckErrors()
		{
			if (errorsCount > 0)
			{
				StopThreads();
				throw new EngineException("There were some errors in test threads. Can't continue.");
			}
		}

		#region Logging
		private void LogTestAlgorithm(TimeSpan duration)
		{
			testLog.Info("Started testing. Duration = {0}.", duration);
		}

		private void LogTopologyBoundaries(int initialCount)
		{
			testLog.Info("Initial topology size: {0}", initialCount);
			testLog.Info("Min topology size: {0}", (int)(initialCount * CompositeMutator.MinNodesCountMultiplier));
			testLog.Info("Max topology size: {0}", (int)(initialCount * CompositeMutator.MaxNodesCountMultiplier));
		}

		private void LogEnabledHBM()
		{
			testLog.Info("Enabled HBM. Waiting for full warmup..");
		}

		private void LogMasterWarmedUp()
		{
			testLog.Info("All nodes have sent HBM to Master.");
		}

		private void LogStoppingThreads()
		{
			testLog.Info("Stopping engine threads..");
		}

		private void LogStoppedThreads()
		{
			testLog.Info("Stopped all engine threads.");
		}

		private void LogResult(AlgorithmTestResult result)
		{
			testLog.Info(result.ToString(true));
		}

		private void LogUnexpectedError(Exception error)
		{
			testLog.Error("An unexpected error occured during test: {0}", error);
		}
		#endregion

		private readonly InitialGridState initialState;
		private readonly ILog baseLog;

		private Topology topology;
		private Master master;
		private CompositeMutator topologyMutator;
		private HeartBeatProcessor heartBeatProcessor;
		private TasksGenerator tasksGenerator;
		private ISchedulerAlgorithm schedulerAlgorithm;
		private GarbageCollector garbageCollector;
		private int errorsCount;
		private ILog testLog;
		private Thread[] engineThreads;

		private Action<IntermediateTestResult, string> onIntermediateResult;
	}
}
