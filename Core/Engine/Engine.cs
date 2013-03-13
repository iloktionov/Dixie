﻿using System;
using System.Diagnostics;
using System.Threading;

namespace Dixie.Core
{
	internal class Engine
	{
		public Engine(InitialGridState initialState )
		{
			this.initialState = initialState;
		}

		public AlgorithmTestResult TestAlgorithm(ISchedulerAlgorithm algorithm, TimeSpan testDuration)
		{
			topology = initialState.Topology.Clone();
			master = new Master(initialState.EngineSettings.DeadabilityThreshold);
			schedulerAlgorithm = algorithm;
			topologyMutator = new CompositeMutator(initialState.RandomSeed, topology.WorkerNodesCount, 
				initialState.EngineSettings.RemoveNodesProbability, 
				initialState.EngineSettings.AddNodesProbability
			);
			heartBeatProcessor = new HeartBeatProcessor(topology, master, initialState.EngineSettings.HeartBeatPeriod);
			tasksGenerator = new TasksGenerator(initialState);

			Thread hbThread = StartHeartBeatsMechanism();
			WaitForMasterStateWarmup();
			Thread mutatorThread = StartTopologyMutations();
			Thread tasksThread = StartTaskGeneration();
			Thread algorithmThread = StartSchedulerAlgorithm();

			var testResult = new AlgorithmTestResult();
			var watch = Stopwatch.StartNew();
			while (watch.Elapsed < testDuration)
			{
				Thread.Sleep(TimeSpan.FromMilliseconds(100));
				testResult.AddIntermediateResult(master.GetTotalWorkDone(), watch.Elapsed);
			}

			Thread.Sleep(testDuration);
			algorithmThread.AbortAndWaitCompleted();
			tasksThread.AbortAndWaitCompleted();
			mutatorThread.AbortAndWaitCompleted();
			hbThread.AbortAndWaitCompleted();
			return testResult;
		}

		private Thread StartHeartBeatsMechanism()
		{
			return ThreadRunner.RunPeriodicAction(heartBeatProcessor.DeliverMessagesAndResponses, TimeSpan.FromMilliseconds(1));
		}

		private void WaitForMasterStateWarmup()
		{
			while (master.AliveNodesCount < topology.WorkerNodesCount)
				Thread.Sleep(10);
		}

		private Thread StartTopologyMutations()
		{
			return ThreadRunner.RunPeriodicAction(() => topologyMutator.Mutate(topology), TimeSpan.FromMilliseconds(10));
		}

		private Thread StartTaskGeneration()
		{
			return ThreadRunner.RunPeriodicAction(() => master.RefillTasksIfNeeded(tasksGenerator), TimeSpan.FromMilliseconds(10));
		}

		private Thread StartSchedulerAlgorithm()
		{
			return ThreadRunner.RunPeriodicAction(() => master.ExecuteSchedulerAlgorithm(schedulerAlgorithm), TimeSpan.FromMilliseconds(10));
		}

		private readonly InitialGridState initialState;
		private Topology topology;
		private Master master;
		private CompositeMutator topologyMutator;
		private HeartBeatProcessor heartBeatProcessor;
		private TasksGenerator tasksGenerator;
		private ISchedulerAlgorithm schedulerAlgorithm;
	}
}
