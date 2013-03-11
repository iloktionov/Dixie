﻿using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	internal class RemoveNodesMutator : ITopologyMutator
	{
		public RemoveNodesMutator(OfflineNodesPool offlineNodesPool, Random random, int minRemainingNodes)
		{
			Preconditions.CheckNotNull(offlineNodesPool, "offlineNodesPool");
			Preconditions.CheckNotNull(random, "random");
			Preconditions.CheckArgument(minRemainingNodes > 0, "minRemainingNodes", "Must be > 0.");
			this.offlineNodesPool = offlineNodesPool;
			this.random = random;
			this.minRemainingNodes = minRemainingNodes;
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
				NodeFailureType failureType = DetermineFailureType();
				if (failureType == NodeFailureType.LongTerm)
					nodeToRemove.StopComputing();
				if (failureType != NodeFailureType.Permanent)
					offlineNodesPool.Put(nodeToRemove, parent, failureType, NodeFailureHelper.GetFailureTime(failureType));
			}
		}

		private NodeFailureType DetermineFailureType()
		{
			return (NodeFailureType) random.Next(1, 4);
		}

		private readonly OfflineNodesPool offlineNodesPool;
		private readonly Random random;
		private readonly int minRemainingNodes;
	}
}