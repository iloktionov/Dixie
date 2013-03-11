﻿namespace Dixie.Core
{
	internal class ReturnNodesMutator : ITopologyMutator
	{
		public ReturnNodesMutator(OfflineNodesPool offlineNodesPool, ITopologyConfigurator configurator)
		{
			Preconditions.CheckNotNull(offlineNodesPool, "offlineNodesPool");
			this.offlineNodesPool = offlineNodesPool;
			this.configurator = configurator;
		}

		public void Mutate(Topology topology)
		{
			foreach (OfflineNodeInfo info in offlineNodesPool.GetNodesReadyForReturn())
			{
				if (info.FailureType == NodeFailureType.LongTerm)
					info.OfflineNode.ResumeComputing();
				topology.AddNode(info.OfflineNode, info.ParentNode, configurator.GenerateLinkLatency());
			}
		}

		private readonly OfflineNodesPool offlineNodesPool;
		private readonly ITopologyConfigurator configurator;
	}
}