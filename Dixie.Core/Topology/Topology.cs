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
	// Сценарий многопоточной работы:
	// 1.) Один поток, осуществляющий добавление/удаление нод + повторное взвешивание ребер.
	// 2.) Один или более потоков, получающих список всех нод, узнающих для них latency.
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

		public IEnumerable<Node> GetWorkerNodesUnsafe()
		{
			return workerNodes.Values;
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

		public bool AddNode(Node newNode, INode parentNode, TimeSpan linkLatency)
		{
			Preconditions.CheckNotNull(newNode, "newNode");
			Preconditions.CheckNotNull(parentNode, "parentNode");
			Preconditions.CheckArgument(linkLatency >= TimeSpan.Zero, "linkLatency", "Latency must be not negative.");
			lock (syncObject)
			{
				CheckNodeNotPresent(newNode);
				if (!IsValidParent(parentNode))
					return false;
				graph.AddVertex(newNode);
				graph.AddEdge(new NetworkLink(newNode, parentNode, linkLatency));
				workerNodes.Add(newNode.Id, newNode);
				workerLatencies.Add(newNode.Id, linkLatency + workerLatencies[parentNode.Id]);
				version++;
				return true;
			}
		}

		public void RemoveNode(Node nodeToRemove, out INode parent)
		{
			Preconditions.CheckNotNull(nodeToRemove, "node");
			lock (syncObject)
			{
				CheckNodeIsPresent(nodeToRemove);
				NetworkLink parentLink = GetParentLink(nodeToRemove);
				var childLinks = GetChildLinks(nodeToRemove);
				parent = parentLink.Target;

				graph.RemoveVertex(nodeToRemove);
				workerNodes.Remove(nodeToRemove.Id);
				workerLatencies.Remove(nodeToRemove.Id);
				// Вместе с нодой удалились рёбра в графе. Нужно соединить детей удаленной ноды с бывшим родителем.
				foreach (NetworkLink childLink in childLinks)
				{
					var newLink = new NetworkLink(childLink.Source, parent, childLink.Latency);
					graph.AddEdge(newLink);
					AdjustWorkerLatencies(newLink, parentLink.Latency.Negate());
				}
				version++;
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

		public Topology Clone()
		{
			var newLatencies = new Dictionary<Guid, TimeSpan>(workerLatencies);
			var newWorkerNodes = workerNodes.ToDictionary(pair => pair.Key, pair => pair.Value.Clone());
			var newMasterNode = new MasterFakeNode();
			var newGraph = new BidirectionalGraph<INode, NetworkLink>();
			newGraph.AddVertex(newMasterNode);

			foreach (INode vertex in graph.Vertices.Where(vertex => !(vertex is MasterFakeNode)))
				newGraph.AddVertex(newWorkerNodes[vertex.Id]);
			foreach (NetworkLink edge in graph.Edges)
			{
				INode source = newWorkerNodes[edge.Source.Id];
				INode target = edge.Target is MasterFakeNode ? (INode) newMasterNode : newWorkerNodes[edge.Target.Id];
				newGraph.AddEdge(new NetworkLink(source, target, edge.Latency));
			}
			return new Topology(newGraph, newWorkerNodes, newLatencies, newMasterNode);
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

		public bool ObserveGraph(long providedVersion, Action<BidirectionalGraph<INode, NetworkLink>> graphAction, out long actualVersion)
		{
			lock (syncObject)
			{
				actualVersion = version;
				if (providedVersion == actualVersion)
					return false;
				graphAction(graph);
				return true;
			}
		}

		public int GetNodeTreeHeight(INode node)
		{
			int height = 0;
			IEnumerable<NetworkLink> outEdges;
			while (graph.TryGetOutEdges(node, out outEdges))
			{
				NetworkLink edge = outEdges.FirstOrDefault();
				if (edge == null)
					return height;
				height++;
				node = edge.Target;
			}
			return height;
		}

		public int GetTreeHeight()
		{
			return workerNodes.Values.Max(node => GetNodeTreeHeight(node));
		}

		internal MasterFakeNode MasterNode
		{
			get { return masterNode; }
		}

		internal BidirectionalGraph<INode, NetworkLink> Graph
		{
			get { return graph; }
		}

		internal Dictionary<Guid, TimeSpan> WorkerLatencies
		{
			get { return workerLatencies; }
		}

		internal int WorkerNodesCount
		{
			get { return workerNodes.Count; }
		}

		private bool IsValidParent(INode parentNode)
		{
			if (ReferenceEquals(parentNode, masterNode))
				return true;
			return parentNode is Node && workerNodes.ContainsKey(parentNode.Id);
		}

		private NetworkLink GetParentLink(Node node)
		{
			IEnumerable<NetworkLink> outEdges;
			return !graph.TryGetOutEdges(node, out outEdges) ? null : outEdges.FirstOrDefault();
		}

		private IEnumerable<NetworkLink> GetChildLinks(Node node)
		{
			IEnumerable<NetworkLink> inEdges;
			return graph.TryGetInEdges(node, out inEdges) ? inEdges.ToList() : new List<NetworkLink>();
		}

		private void AdjustWorkerLatencies(NetworkLink newLink, TimeSpan amount)
		{
			workerLatencies[newLink.Source.Id] += amount;
			foreach (NetworkLink childLink in GetChildLinks((Node)newLink.Source))
				AdjustWorkerLatencies(childLink, amount);
		}

		private void CheckNodeNotPresent(Node node)
		{
			if (workerNodes.ContainsKey(node.Id))
				throw new InvalidOperationException(String.Format("Node with id {0} is already in topology.", node.Id));
		}

		private void CheckNodeIsPresent(Node node)
		{
			if (!workerNodes.ContainsKey(node.Id))
				throw new InvalidOperationException(String.Format("Node with id {0} not found in topology.", node.Id));
		}

		private readonly BidirectionalGraph<INode, NetworkLink> graph;
		private readonly Dictionary<Guid, Node> workerNodes;
		private readonly Dictionary<Guid, TimeSpan> workerLatencies;
		private readonly MasterFakeNode masterNode;
		private readonly object syncObject;
		private long version;
	}
}