using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	public class TopologyBuilder
	{
		public TopologyBuilder(int minChildren, int maxChildren)
		{
			this.minChildren = minChildren;
			this.maxChildren = maxChildren;
			random = new Random();
		}

		public Topology Build(int nodesCount)
		{
			Topology topology = Topology.CreateEmpty();
			int nodesGenerated = 0;
			IEnumerable<INode> previousLayer = new[] { topology.MasterNode };
			while (nodesGenerated < nodesCount)
				previousLayer = GenerateLayer(topology, previousLayer, nodesCount, ref nodesGenerated);
			return topology;
		}

		private IEnumerable<Node> GenerateLayer(Topology topology, IEnumerable<INode> previousLayer, int nodesCount, ref int nodesGenerated)
		{
			var newLayer = new List<Node>();
			foreach (INode parentNode in previousLayer)
			{
				int childrenCount = random.Next(minChildren, maxChildren + 1);
				for (int i = 0; i < childrenCount; i++)
				{
					// TODO(iloktionov): select performace randomly
					var newNode = new Node(1, 1);
					topology.AddNode(newNode, parentNode, TimeSpan.FromMilliseconds(1));
					newLayer.Add(newNode);
					nodesGenerated++;
					if (nodesGenerated >= nodesCount)
						return newLayer;
				}
			}
			return newLayer;
		}

		private readonly int minChildren;
		private readonly int maxChildren;
		private readonly Random random;
	}
}