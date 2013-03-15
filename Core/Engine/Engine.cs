using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace Dixie.Core
{
	public class Engine
	{
		public Engine(InitialGridState initialState, ILog log)
		{
			this.initialState = initialState;
			this.log = log;
		}

		public ComparisonTestResult TestAlgorithms(IEnumerable<ISchedulerAlgorithm> algorithms, TimeSpan testDuration, TimeSpan intermediateCheckPeriod)
		{
			var result = new ComparisonTestResult();
			foreach (ISchedulerAlgorithm algorithm in algorithms)
				result.AddAlgorithmResult(algorithm, TestAlgorithm(algorithm, testDuration, intermediateCheckPeriod));
			return result;
		}

		public AlgorithmTestResult TestAlgorithm(ISchedulerAlgorithm algorithm, TimeSpan testDuration, TimeSpan intermediateCheckPeriod)
		{
			topology = initialState.Topology.Clone();
			master = new Master(initialState.EngineSettings.DeadabilityThreshold);
			schedulerAlgorithm = algorithm;
			garbageCollector = new GarbageCollector();
			topologyMutator = new CompositeMutator(initialState.RandomSeed, topology.WorkerNodesCount, 
				initialState.EngineSettings.RemoveNodesProbability, 
				initialState.EngineSettings.AddNodesProbability, 
				initialState.TopologySettings
			);
			heartBeatProcessor = new HeartBeatProcessor(topology, master, initialState.EngineSettings.HeartBeatPeriod);
			tasksGenerator = new TasksGenerator(initialState, log);

			var engineThreads = new Thread[5];
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
			}
			// (iloktionov): Теперь остановим подсчет результатов и возьмем финальное значение перед завершением потоков.
			master.DisableAccumulatingResults();
			testResult.AddIntermediateResult(master.GetTotalWorkDone(), watch.Elapsed);
			ThreadRunner.StopThreads(engineThreads);
			return testResult;
		}

		private Thread StartHeartBeatsMechanism(WaitHandle syncEvent)
		{
			return ThreadRunner.RunPeriodicAction(heartBeatProcessor.DeliverMessagesAndResponses, TimeSpan.FromMilliseconds(1), syncEvent);
		}

		private void WaitForMasterStateWarmup()
		{
			while (master.AliveNodesCount < topology.WorkerNodesCount)
				Thread.Sleep(10);
		}

		private Thread StartTopologyMutations(WaitHandle syncEvent)
		{
			return ThreadRunner.RunPeriodicAction(() => topologyMutator.Mutate(topology), initialState.EngineSettings.TopologyMutatorRunPeriod, syncEvent);
		}

		private Thread StartTaskGeneration(WaitHandle syncEvent)
		{
			return ThreadRunner.RunPeriodicAction(() => master.RefillTasksIfNeeded(tasksGenerator), initialState.EngineSettings.TasksGeneratorRunPeriod, syncEvent);
		}

		private Thread StartSchedulerAlgorithm(WaitHandle syncEvent)
		{
			return ThreadRunner.RunPeriodicAction(() => master.ExecuteSchedulerAlgorithm(schedulerAlgorithm), initialState.EngineSettings.SchedulingAlgorithmRunPeriod, syncEvent);
		}

		private Thread StartGarbageCollection(WaitHandle syncEvent)
		{
			return ThreadRunner.RunPeriodicAction(() => garbageCollector.CollectGarbage(master, heartBeatProcessor), initialState.EngineSettings.GarbageCollectorRunPeriod, syncEvent);
		}

		private readonly InitialGridState initialState;
		private readonly ILog log;

		private Topology topology;
		private Master master;
		private CompositeMutator topologyMutator;
		private HeartBeatProcessor heartBeatProcessor;
		private TasksGenerator tasksGenerator;
		private ISchedulerAlgorithm schedulerAlgorithm;
		private GarbageCollector garbageCollector;
	}
}
