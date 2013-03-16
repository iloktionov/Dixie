using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	public class TopologyBuilder
	{
		internal TopologyBuilder(ITopologyConfigurator configurator, Random random)
		{
			this.configurator = configurator;
			this.random = random;
		}

		public TopologyBuilder(Random random) 
			: this (new TopologyConfigurator(), random) { }

		public TopologyBuilder() 
			: this (new Random()) { }

		public Topology Build(int nodesCount)
		{
			int childrenCount = Math.Max(1, (int) Math.Log(nodesCount, Math.Sqrt(nodesCount) / 2));
			Topology topology = Topology.CreateEmpty();
			int nodesGenerated = 0;
			IEnumerable<INode> previousLayer = new[] { topology.MasterNode };
			while (nodesGenerated < nodesCount)
				previousLayer = GenerateLayer(topology, previousLayer, nodesCount, childrenCount, ref nodesGenerated);
			return topology;
		}

		private IEnumerable<Node> GenerateLayer(Topology topology, IEnumerable<INode> previousLayer, int nodesCount, int childrenCount, ref int nodesGenerated)
		{
			var newLayer = new List<Node>();
			foreach (INode parentNode in previousLayer)
			{
				for (int i = 0; i < childrenCount; i++)
				{
					var newNode = new Node(configurator.GeneratePerformance(), configurator.GenerateFailureProbability(), NodeFailurePattern.Generate(random));
					topology.AddNode(newNode, parentNode, configurator.GenerateLinkLatency());
					newLayer.Add(newNode);
					nodesGenerated++;
					if (nodesGenerated >= nodesCount)
						return newLayer;
				}
			}
			return newLayer;
		}

		private readonly ITopologyConfigurator configurator;
		private readonly Random random;
	}
}