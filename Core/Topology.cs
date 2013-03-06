using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using QuickGraph;
using QuickGraph.Serialization;

namespace Dixie.Core
{
	// Concurrency scenario:
	// 1.) Thread that adds/removes nodes, reweights edges
	// 2.) Thread that forwards HB messages and responses

	public partial class Topology
	{
		public Topology(BidirectionalGraph<INode, NetworkLink> graph, Dictionary<Guid, Node> workerNodes, Dictionary<Guid, TimeSpan> workerLatencies, MasterFakeNode masterNode)
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
		/// <returns>True on success; false if parent didn't exist.</returns>
		public bool AddNode(Node newNode, Node parentNode, TimeSpan linkLatency)
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
						return false;
					parent = parentNode;
				}
				graph.AddVertex(newNode);
				graph.AddEdge(new NetworkLink(newNode, parent, linkLatency));
				workerNodes.Add(newNode.Id, newNode);
				workerLatencies.Add(newNode.Id, linkLatency + workerLatencies[parent.Id]);
				return true;
			}
		}

		public void RemoveNode(Node node, out INode parent)
		{
			if (node == null)
				throw new ArgumentNullException("node");
			lock (syncObject)
			{
				if (!workerNodes.ContainsKey(node.Id))
					throw new InvalidOperationException(String.Format("Node with id {0} not found in topology.", node.Id));
				NetworkLink parentLink = GetParentLink(node);
				var childLinks = GetChildLinks(node);
				parent = parentLink.Target;

				graph.RemoveVertex(node);
				workerNodes.Remove(node.Id);
				workerLatencies.Remove(node.Id);
				// Вместе с нодой удалились рёбра в графе. Нужно соединить детей удаленной ноды с бывшим родителем.
				foreach (NetworkLink childLink in childLinks)
					graph.AddEdge(new NetworkLink(childLink.Source, parent, childLink.Latency));
				// TODO: adjust children latencies
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
			var graph = stream.DeserializeFromBinary<INode, NetworkLink, BidirectionalGraph<INode, NetworkLink>>();
			var workerNodes = (Dictionary<Guid, Node>) formatter.Deserialize(stream);
			var workerLatencies = (Dictionary<Guid, TimeSpan>) formatter.Deserialize(stream);
			var masterNode = (MasterFakeNode) formatter.Deserialize(stream);
			return new Topology(graph, workerNodes, workerLatencies, masterNode);
		}

		public static Topology CreateEmpty()
		{
			var graph = new BidirectionalGraph<INode, NetworkLink>();
			var workerNodes = new Dictionary<Guid, Node>();
			var workerLatencies = new Dictionary<Guid, TimeSpan>();
			var masterNode = new MasterFakeNode();
			graph.AddVertex(masterNode);
			workerLatencies.Add(masterNode.Id, TimeSpan.Zero);
			return new Topology(graph, workerNodes, workerLatencies, masterNode);
		}

		public override string ToString()
		{
			var builder = new StringBuilder();
			builder.AppendLine("Nodes:");
			builder.Append(String.Join(Environment.NewLine, workerNodes.Keys));
			builder.AppendLine();
			builder.AppendLine("Edges:");
			builder.Append(String.Join(Environment.NewLine, graph.Edges));
			return builder.ToString();
		}

		internal BidirectionalGraph<INode, NetworkLink> Graph
		{
			get { return graph; }
		}

		private NetworkLink GetParentLink(Node node)
		{
			IEnumerable<NetworkLink> outEdges;
			if (!graph.TryGetOutEdges(node, out outEdges))
				return null;
			return outEdges.FirstOrDefault();
		}

		private IEnumerable<NetworkLink> GetChildLinks(Node node)
		{
			IEnumerable<NetworkLink> inEdges;
			return graph.TryGetInEdges(node, out inEdges) ? inEdges.ToList() : new List<NetworkLink>();
		} 

		private readonly BidirectionalGraph<INode, NetworkLink> graph;
		private readonly Dictionary<Guid, Node> workerNodes;
		private readonly Dictionary<Guid, TimeSpan> workerLatencies;
		private readonly MasterFakeNode masterNode;
		private readonly object syncObject;
	}
}