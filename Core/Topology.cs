using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using QuickGraph;
using QuickGraph.Serialization;

namespace Dixie.Core
{
	// Concurrency scenario:
	// 1.) Thread that adds/removes nodes, reweights edges
	// 2.) Thread that forwards HB messages and responses

	public class Topology
	{
		public Topology(AdjacencyGraph<INode, NetworkLink> graph, Dictionary<Guid, Node> workerNodes, Dictionary<Guid, TimeSpan> workerLatencies, MasterFakeNode masterNode)
		{
			this.graph = graph;
			this.workerNodes = workerNodes;
			this.workerLatencies = workerLatencies;
			this.masterNode = masterNode;
			syncObject = new object();
		}

		public List<Node> GetWorkerNodes()
		{
			lock (syncObject)
				return new List<Node>(workerNodes.Values);
		} 

		public bool TryGetNode(Guid nodeId, out Node node)
		{
			lock (syncObject)
				return workerNodes.TryGetValue(nodeId, out node);
		}

		public bool TryGetNodeLatency(Guid nodeId, out TimeSpan latency)
		{
			lock (syncObject)
				return workerLatencies.TryGetValue(nodeId, out latency);
		}

		/// <summary>
		/// Adds a new node to topology and connects it with parent node.
		/// </summary>
		/// <param name="newNode"></param>
		/// <param name="parentNode">Any of existing nodes. Null is treated like a master-node.</param>
		/// <param name="linkLatency"></param>
		public void AddNode(Node newNode, Node parentNode, TimeSpan linkLatency)
		{
			if (newNode == null)
				throw new ArgumentNullException("newNode");
			if (linkLatency < TimeSpan.Zero)
				throw new ArgumentOutOfRangeException("linkLatency", "Latency must be non-negative.");

			lock (syncObject)
			{
				if (workerNodes.ContainsKey(newNode.Id))
					throw new InvalidOperationException(String.Format("Node with id {0} is already in topology.", newNode.Id));
				INode parent;
				if (parentNode == null)
					parent = masterNode;
				else
				{
					if (!workerNodes.ContainsKey(parentNode.Id))
						throw new InvalidOperationException(String.Format("Parent node with id {0} not found in topology.", parentNode.Id));
					parent = parentNode;
				}
				graph.AddVertex(newNode);
				graph.AddEdge(new NetworkLink(newNode, parent, linkLatency));
				workerNodes.Add(newNode.Id, newNode);
				workerLatencies.Add(newNode.Id, linkLatency + workerLatencies[parent.Id]);
			}
		}

		public void Serialize(Stream stream)
		{
			var formatter = new BinaryFormatter();
			graph.SerializeToBinary(stream);
			formatter.Serialize(stream, workerNodes);
			formatter.Serialize(stream, workerLatencies);
			formatter.Serialize(stream, masterNode);
		}

		public static Topology Deserialize(Stream stream)
		{
			var formatter = new BinaryFormatter();
			var graph = stream.DeserializeFromBinary<INode, NetworkLink, AdjacencyGraph<INode, NetworkLink>>();
			var workerNodes = (Dictionary<Guid, Node>) formatter.Deserialize(stream);
			var workerLatencies = (Dictionary<Guid, TimeSpan>) formatter.Deserialize(stream);
			var masterNode = (MasterFakeNode) formatter.Deserialize(stream);
			return new Topology(graph, workerNodes, workerLatencies, masterNode);
		}

		public static Topology CreateEmpty()
		{
			var graph = new AdjacencyGraph<INode, NetworkLink>();
			var workerNodes = new Dictionary<Guid, Node>();
			var workerLatencies = new Dictionary<Guid, TimeSpan>();
			var masterNode = new MasterFakeNode();
			graph.AddVertex(masterNode);
			workerLatencies.Add(masterNode.Id, TimeSpan.Zero);
			return new Topology(graph, workerNodes, workerLatencies, masterNode);
		}

		private readonly AdjacencyGraph<INode, NetworkLink> graph;
		private readonly Dictionary<Guid, Node> workerNodes;
		private readonly Dictionary<Guid, TimeSpan> workerLatencies;
		private readonly MasterFakeNode masterNode;
		private readonly object syncObject;
	}
}