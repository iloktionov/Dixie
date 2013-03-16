using System;
using System.Collections.Generic;
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
			INode[] parents = SelectParents(topology, nodesToAdd);
			for (int i = 0; i < nodesToAdd; i++)
			{
				var newNode = new Node(configurator.GeneratePerformance(), configurator.GenerateFailureProbability(), NodeFailurePattern.Generate(random));
				topology.AddNode(newNode, parents[i], configurator.GenerateLinkLatency());
			}
		}

		private static INode[] SelectParents(Topology topology, int count)
		{
			var potentialParents = topology.Graph.Vertices
				.Select(vertex => new KeyValuePair<INode, int>(vertex, GetTreeChildrenCount(topology, vertex)))
				.OrderBy(pair => pair.Value)
				.ToList();
			var result = new INode[count];
			int parentIndex = 0;
			for (int i = 0; i < count; i++)
			{
				result[i] = potentialParents[parentIndex].Key;
				parentIndex++;
				parentIndex = parentIndex % potentialParents.Count;
			}
			return result;
		}

		private static int GetTreeChildrenCount(Topology topology, INode vertex)
		{
			IEnumerable<NetworkLink> inEdges;
			if (!topology.Graph.TryGetInEdges(vertex, out inEdges))
				return 0;
			return inEdges.Count();
		}

		private readonly Random random;
		private readonly int maxRemainingNodes;
		private readonly ITopologyConfigurator configurator;
	}
}