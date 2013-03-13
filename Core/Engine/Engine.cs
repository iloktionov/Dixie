using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Dixie.Core
{
	internal class Engine
	{
		public Engine(InitialGridState initialState )
		{
			this.initialState = initialState;
		}

		private void TestAlgorithm(ISchedulerAlgorithm algorithm, TimeSpan testDuration)
		{
			topology = initialState.Topology.Clone();
			master = new Master();
			schedulerAlgorithm = algorithm;
			topologyMutator = new CompositeMutator(initialState.RandomSeed, topology.WorkerNodesCount, 
				initialState.EngineSettings.RemoveNodesProbability, 
				initialState.EngineSettings.AddNodesProbability
			);
			heartBeatProcessor = new HeartBeatProcessor(topology, master, initialState.EngineSettings.HeartBeatPeriod);

			Thread hbThread = StartHeartBeatsMechanism();
			WaitForMasterStateWarmup();
			Thread mutatorThread = StartTopologyMutations();
			Thread algorithmThread = StartSchedulerAlgorithm();
			Thread.Sleep(testDuration);
		}

		private Thread StartHeartBeatsMechanism()
		{
			return ThreadRunner.RunPeriodicAction(heartBeatProcessor.DeliverMessagesAndResponses, TimeSpan.FromMilliseconds(1));
		}

		private Thread StartSchedulerAlgorithm()
		{
			return ThreadRunner.RunPeriodicAction(() => master.ExecuteSchedulerAlgorithm(schedulerAlgorithm), TimeSpan.FromMilliseconds(10));
		}

		private Thread StartTopologyMutations()
		{
			return ThreadRunner.RunPeriodicAction(() => topologyMutator.Mutate(topology), TimeSpan.FromMilliseconds(10));
		}

		private void WaitForMasterStateWarmup()
		{
			while (master.AliveNodesCount < topology.WorkerNodesCount)
				Thread.Sleep(10);
		}

		private readonly InitialGridState initialState;
		private Topology topology;
		private Master master;
		private CompositeMutator topologyMutator;
		private HeartBeatProcessor heartBeatProcessor;
		private ISchedulerAlgorithm schedulerAlgorithm;
	}
}
