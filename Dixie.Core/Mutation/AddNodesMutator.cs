using System;
using System.Linq;

namespace Dixie.Core
{
	internal class AddNodesMutator : ITopologyMutator
	{
		public AddNodesMutator(Random random, ITopologyConfigurator configurator, int maxRemainingNodes)
		{
			Preconditions.CheckNotNull(random, "random");
			Preconditions.CheckNotNull(configurator, "configurator");
			Preconditions.CheckArgument(maxRemainingNodes > 0, "maxRemainingNodes", "Must be > 0.");
			this.random = random;
			this.configurator = configurator;
			this.maxRemainingNodes = maxRemainingNodes;
		}

		public void Mutate(Topology topology)
		{
			if (topology.WorkerNodesCount >= maxRemainingNodes)
				return;
			int nodesToAdd = random.Next(0, maxRemainingNodes - topology.WorkerNodesCount + 1);
			INode[] parents = SelectRandomParents(topology, nodesToAdd);
			for (int i = 0; i < nodesToAdd; i++)
			{
				var newNode = new Node(configurator.GeneratePerformance(), configurator.GenerateFailureProbability(), NodeFailurePattern.Generate(random));
				topology.AddNode(newNode, parents[i], configurator.GenerateLinkLatency());
			}
		}

		private INode[] SelectRandomParents(Topology topology, int count)
		{
			INode[] potentialParents = topology.Graph.Vertices
				.Except(new [] {topology.MasterNode})
				.ToArray();
			var result = new INode[count];
			int parentsDone = 0;
			while (parentsDone < count)
			{
				INode parent = potentialParents[random.Next(potentialParents.Length)];
				int parentOrder = random.Next(count - parentsDone + 1);
				for (int i = 0; i < parentOrder; i++)
					result[parentsDone++] = parent;
			}
			return result;
		}

		private readonly Random random;
		private readonly int maxRemainingNodes;
		private readonly ITopologyConfigurator configurator;
	}
}