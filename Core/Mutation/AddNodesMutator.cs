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
				var newNode = new Node(configurator.GeneratePerformance(), configurator.GenerateFailureProbability());
				topology.AddNode(newNode, parents[i], configurator.GenerateLinkLatency());
			}
		}

		private INode[] SelectRandomParents(Topology topology, int count)
		{
			var allNodesList = topology.Graph.Vertices.ToArray();
			var result = new INode[count];
			for (int i = 0; i < count; i++)
				result[i] = allNodesList[random.Next(0, allNodesList.Length)];
			return result;
		}

		private readonly Random random;
		private readonly int maxRemainingNodes;
		private readonly ITopologyConfigurator configurator;
	}
}