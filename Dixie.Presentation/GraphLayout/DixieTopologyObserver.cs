﻿using System;
using System.Collections.Generic;
using Dixie.Core;

namespace Dixie.Presentation
{
	internal class DixieTopologyObserver
	{
		public DixieTopologyObserver(DixieModel model)
		{
			this.model = model;
			version = -1;
		}

		public void TryUpdateModelGraph(Topology topology)
		{
			if (!PresentationSettings.GetInstance().EnableTopologyRendering)
				return;
			DixieGraph displayGraph = null;
			long actualVersion;
			// (iloktionov): Построение графа выполняется под локом на уровне Topology.
			bool result = topology.ObserveGraph(version, 
				graph =>
					{
						displayGraph = new DixieGraph();
						var nodesMap = new Dictionary<Guid, INode>();
						foreach (INode node in graph.Vertices)
						{
							if (node is MasterFakeNode)
							{
								nodesMap.Add(node.Id, node);
								displayGraph.AddVertex(node);
							}
							else
							{
								NodeState nodeState = ((Node)node).GetState();
								nodesMap.Add(node.Id, nodeState);
								displayGraph.AddVertex(nodeState);
							}
						}
						foreach (NetworkLink edge in graph.Edges)
						{
							INode newSource = nodesMap[edge.Source.Id];
							INode newTarget = nodesMap[edge.Target.Id];
							displayGraph.AddEdge(new NetworkLink(newSource, newTarget, edge.Latency));
						}
					},
				out actualVersion);
			version = actualVersion;
			if (result)
				model.TopologyGraph = displayGraph;
		}

		public void Reset()
		{
			model.TopologyGraph = null;
			version = -1;
		}

		private readonly DixieModel model;
		private long version;
	}
}