using System;

namespace Dixie.Core
{
	// TODO(iloktionov): reweight topology edges
	// TODO(iloktionov): optimize speed
	internal class CompositeMutator : ITopologyMutator
	{
		public CompositeMutator(int seed, int initialNodesCount, double removeNodesProbability, double addNodesProbability, TopologySettings topologySettings)
		{
			this.removeNodesProbability = removeNodesProbability;
			this.addNodesProbability = addNodesProbability;
			random = new Random(seed);
			configurator = new TopologyConfigurator(topologySettings, random);
			var offlinePool = new OfflineNodesPool();
			removeMutator = new RemoveNodesMutator(offlinePool, random, configurator, Math.Max(1, (int)(initialNodesCount * MinNodesCountMultiplier)));
			returnMutator = new ReturnNodesMutator(offlinePool, configurator);
			addMutator = new AddNodesMutator(random, configurator, (int)(initialNodesCount * MaxNodesCountMultiplier));
		}

		public void Mutate(Topology topology)
		{
			if (random.TemptProvidence(removeNodesProbability))
				removeMutator.Mutate(topology);
			else if (random.TemptProvidence(addNodesProbability))
				addMutator.Mutate(topology);
			returnMutator.Mutate(topology);
		}

		private readonly Random random;
		private readonly ITopologyConfigurator configurator;
		private readonly ITopologyMutator removeMutator;
		private readonly ITopologyMutator returnMutator;
		private readonly ITopologyMutator addMutator;
		private readonly double removeNodesProbability;
		private readonly double addNodesProbability;

		private const double MinNodesCountMultiplier = 0.7d;
		private const double MaxNodesCountMultiplier = 1.15d;
	}
}