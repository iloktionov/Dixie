using System;

namespace Dixie.Core
{
	internal class CompositeMutator : ITopologyMutator
	{
		public CompositeMutator(int seed, int initialNodesCount, double removeNodesProbability, double addNodesProbability)
		{
			this.removeNodesProbability = removeNodesProbability;
			this.addNodesProbability = addNodesProbability;
			random = new Random(seed);
			configurator = new TopologyConfigurator(random);
			var offlinePool = new OfflineNodesPool();
			removeMutator = new RemoveNodesMutator(offlinePool, random, configurator, Math.Max(1, (int)(initialNodesCount * 0.7d)));
			returnMutator = new ReturnNodesMutator(offlinePool, configurator);
			addMutator = new AddNodesMutator(random, configurator, (int)(initialNodesCount * 1.3d));
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
		private readonly RemoveNodesMutator removeMutator;
		private readonly ReturnNodesMutator returnMutator;
		private readonly AddNodesMutator addMutator;

		private readonly double removeNodesProbability;
		private readonly double addNodesProbability;
	}
}