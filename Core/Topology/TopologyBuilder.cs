using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	public class TopologyBuilder
	{
		public TopologyBuilder(ITopologyConfigurator configurator)
		{
			this.configurator = configurator;
		}

		public TopologyBuilder() : this (TopologyConfigurator.CreateDefault()) { }

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
				int childrenCount = configurator.GenerateChildrenCount();
				for (int i = 0; i < childrenCount; i++)
				{
					var newNode = new Node(configurator.GeneratePerformance(), configurator.GenerateFailureProbability());
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
	}
}