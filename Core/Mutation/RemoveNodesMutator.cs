using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	internal class RemoveNodesMutator : ITopologyMutator
	{
		public RemoveNodesMutator(OfflineNodesPool offlineNodesPool, Random random, ITopologyConfigurator configurator, int minRemainingNodes, GarbageCollector garbageCollector)
		{
			Preconditions.CheckNotNull(offlineNodesPool, "offlineNodesPool");
			Preconditions.CheckNotNull(random, "random");
			Preconditions.CheckNotNull(garbageCollector, "garbageCollector");
			Preconditions.CheckNotNull(configurator, "configurator");
			Preconditions.CheckArgument(minRemainingNodes > 0, "minRemainingNodes", "Must be > 0.");
			this.offlineNodesPool = offlineNodesPool;
			this.random = random;
			this.configurator = configurator;
			this.minRemainingNodes = minRemainingNodes;
			this.garbageCollector = garbageCollector;
		}

		public void Mutate(Topology topology)
		{
			List<Node> nodesToRemove = null;
			int remainingNodes = topology.WorkerNodesCount;
			foreach (Node node in topology.GetWorkerNodesUnsafe())
			{
				if (remainingNodes <= minRemainingNodes)
					break;
				if (random.TemptProvidence(node.FailureProbability))
				{
					if (nodesToRemove == null)
						nodesToRemove = new List<Node>();
					nodesToRemove.Add(node);
					remainingNodes--;
				}
			}
			if (nodesToRemove == null)
				return;
			foreach (Node nodeToRemove in nodesToRemove)
			{
				INode parent;
				topology.RemoveNode(nodeToRemove, out parent);
				NodeFailureType failureType = nodeToRemove.GetFailureType(random);
				if (failureType == NodeFailureType.LongTerm)
					nodeToRemove.StopComputing();
				if (failureType != NodeFailureType.Permanent)
					offlineNodesPool.Put(nodeToRemove, parent, failureType, configurator.GenerateOfflineTime(failureType));
				else garbageCollector.AddStaleNode(nodeToRemove.Id);
			}
		}

		private readonly OfflineNodesPool offlineNodesPool;
		private readonly Random random;
		private readonly ITopologyConfigurator configurator;
		private readonly int minRemainingNodes;
		private readonly GarbageCollector garbageCollector;
	}
}