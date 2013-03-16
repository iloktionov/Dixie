namespace Dixie.Core
{
	internal class ReturnNodesMutator : ITopologyMutator
	{
		public ReturnNodesMutator(OfflineNodesPool offlineNodesPool, ITopologyConfigurator configurator)
		{
			Preconditions.CheckNotNull(offlineNodesPool, "offlineNodesPool");
			Preconditions.CheckNotNull(configurator, "configurator");
			this.offlineNodesPool = offlineNodesPool;
			this.configurator = configurator;
		}

		public void Mutate(Topology topology)
		{
			foreach (OfflineNodeInfo info in offlineNodesPool.PopNodesReadyForReturn())
			{
				if (info.FailureType == NodeFailureType.LongTerm)
					info.OfflineNode.ResumeComputing();
				// (iloktionov): Добавление может и не произойти, если parent уже не в топологии, но ничего страшного в этом нет. 
				topology.AddNode(info.OfflineNode, info.ParentNode, configurator.GenerateLinkLatency());
			}
		}

		private readonly OfflineNodesPool offlineNodesPool;
		private readonly ITopologyConfigurator configurator;
	}
}