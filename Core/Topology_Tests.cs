using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Dixie.Core
{
	public partial class Topology
	{
		[TestFixture]
		internal class Topology_Tests
		{
			[Test]
			public void Test_AddNode()
			{
				Topology topology = CreateEmpty();
				Assert.Throws<ArgumentNullException>(() => topology.AddNode(null, null, TimeSpan.FromMilliseconds(1)));
				Assert.Throws<ArgumentException>(() => topology.AddNode(new Node(0, 0), topology.masterNode, TimeSpan.FromMilliseconds(1).Negate()));
				// Добавим новую ноду к мастеру.
				Assert.True(topology.AddNode(node1, topology.masterNode, TimeSpan.FromMilliseconds(1)));
				// Нельзя добавить одну ноду дважды.
				Assert.Throws<InvalidOperationException>(() => topology.AddNode(node1, topology.masterNode, TimeSpan.FromMilliseconds(1)));
				// Нельзя добавить ноду к несуществующему родителю.
				Assert.False(topology.AddNode(node2, node3, TimeSpan.FromMilliseconds(1)));
				// Но можно к существующему (не мастеру).
				Assert.True(topology.AddNode(node2, node1, TimeSpan.FromMilliseconds(1)));
				Assert.True(topology.AddNode(node3, node1, TimeSpan.FromMilliseconds(1)));
			}

			[Test]
			public void Test_RemoveNode()
			{
				Topology topology = CreateEmpty();
				topology.AddNode(node1, topology.masterNode, TimeSpan.FromMilliseconds(1));
				topology.AddNode(node2, topology.masterNode, TimeSpan.FromMilliseconds(2));
				topology.AddNode(node3, node1, TimeSpan.FromMilliseconds(3));
				topology.AddNode(node4, node1, TimeSpan.FromMilliseconds(4));
				topology.AddNode(node5, node4, TimeSpan.FromMilliseconds(5));
				PrintTopology(topology);
				Assert.True(NodesAreConnected(topology, node1, topology.masterNode));
				Assert.True(NodesAreConnected(topology, node2, topology.masterNode));
				Assert.True(NodesAreConnected(topology, node3, node1));
				Assert.True(NodesAreConnected(topology, node4, node1));
				Assert.True(NodesAreConnected(topology, node5, node4));

				topology.RemoveNode(node4, out parent);
				PrintTopology(topology);
				Assert.AreEqual(5, topology.graph.VertexCount);
				Assert.True(ReferenceEquals(parent, node1));
				Assert.True(NodesAreConnected(topology, node1, topology.masterNode));
				Assert.True(NodesAreConnected(topology, node2, topology.masterNode));
				Assert.True(NodesAreConnected(topology, node3, node1));
				Assert.True(NodesAreConnected(topology, node5, node1));

				topology.RemoveNode(node1, out parent);
				PrintTopology(topology);
				Assert.AreEqual(4, topology.graph.VertexCount);
				Assert.True(ReferenceEquals(parent, topology.masterNode));
				Assert.True(NodesAreConnected(topology, node2, topology.masterNode));
				Assert.True(NodesAreConnected(topology, node3, topology.masterNode));
				Assert.True(NodesAreConnected(topology, node5, topology.masterNode));

				topology.RemoveNode(node2, out parent);
				topology.RemoveNode(node3, out parent);
				topology.RemoveNode(node5, out parent);
				PrintTopology(topology);
				Assert.AreEqual(1, topology.graph.VertexCount);
				Assert.AreEqual(0, topology.graph.EdgeCount);
			}

			[Test]
			public void Test_NodeLatencies()
			{
				Topology topology = CreateEmpty();
				topology.AddNode(node1, topology.masterNode, TimeSpan.FromMilliseconds(1));
				topology.AddNode(node2, topology.masterNode, TimeSpan.FromMilliseconds(2));
				topology.AddNode(node3, node1, TimeSpan.FromMilliseconds(3));
				topology.AddNode(node4, node1, TimeSpan.FromMilliseconds(4));
				topology.AddNode(node5, node4, TimeSpan.FromMilliseconds(5));
				topology.AddNode(node6, node5, TimeSpan.FromMilliseconds(6));
				topology.AddNode(node7, node6, TimeSpan.FromMilliseconds(7));
				PrintTopology(topology);
				CheckLatency(topology, node1, TimeSpan.FromMilliseconds(1));
				CheckLatency(topology, node2, TimeSpan.FromMilliseconds(2));
				CheckLatency(topology, node3, TimeSpan.FromMilliseconds(4));
				CheckLatency(topology, node4, TimeSpan.FromMilliseconds(5));
				CheckLatency(topology, node5, TimeSpan.FromMilliseconds(10));
				CheckLatency(topology, node6, TimeSpan.FromMilliseconds(16));
				CheckLatency(topology, node7, TimeSpan.FromMilliseconds(23));

				topology.RemoveNode(node4, out parent);
				CheckLatency(topology, node1, TimeSpan.FromMilliseconds(1));
				CheckLatency(topology, node2, TimeSpan.FromMilliseconds(2));
				CheckLatency(topology, node3, TimeSpan.FromMilliseconds(4));
				CheckLatency(topology, node5, TimeSpan.FromMilliseconds(6));
				CheckLatency(topology, node6, TimeSpan.FromMilliseconds(12));
				CheckLatency(topology, node7, TimeSpan.FromMilliseconds(19));

				topology.RemoveNode(node1, out parent);
				CheckLatency(topology, node2, TimeSpan.FromMilliseconds(2));
				CheckLatency(topology, node3, TimeSpan.FromMilliseconds(3));
				CheckLatency(topology, node5, TimeSpan.FromMilliseconds(5));
				CheckLatency(topology, node6, TimeSpan.FromMilliseconds(11));
				CheckLatency(topology, node7, TimeSpan.FromMilliseconds(18));

				topology.RemoveNode(node2, out parent);
				topology.RemoveNode(node7, out parent);
				CheckLatency(topology, node3, TimeSpan.FromMilliseconds(3));
				CheckLatency(topology, node5, TimeSpan.FromMilliseconds(5));
				CheckLatency(topology, node6, TimeSpan.FromMilliseconds(11));

				topology.RemoveNode(node5, out parent);
				CheckLatency(topology, node3, TimeSpan.FromMilliseconds(3));
				CheckLatency(topology, node6, TimeSpan.FromMilliseconds(6));

				topology.RemoveNode(node3, out parent);
				topology.RemoveNode(node6, out parent);
				Assert.AreEqual(1, topology.workerLatencies.Count);
			}

			[Test]
			public void Test_Serialization()
			{
				Topology topology = CreateEmpty();
				topology.AddNode(node1, topology.masterNode, TimeSpan.FromMilliseconds(1));
				topology.AddNode(node2, topology.masterNode, TimeSpan.FromMilliseconds(1));
				topology.AddNode(node3, topology.masterNode, TimeSpan.FromMilliseconds(1));

				var stream = new MemoryStream();
				topology.Serialize(stream);
				stream.Seek(0, SeekOrigin.Begin);
				Topology deserializedTopology = Deserialize(stream);
				Assert.True(deserializedTopology.GetWorkerNodes().SequenceEqual(topology.GetWorkerNodes()));
			}

			private static void PrintTopology(Topology topology)
			{
				Console.Out.WriteLine();
				Console.Out.WriteLine(topology);
			}

			private static bool NodesAreConnected(Topology topology, INode child, INode parent)
			{
				return topology.graph.Edges.Any(link => link.Source.Equals(child) && link.Target.Equals(parent));
			}

			private static void CheckLatency(Topology topology, Node node, TimeSpan expectedLatency)
			{
				TimeSpan actualLatency;
				Assert.True(topology.TryGetNodeLatency(node.Id, out actualLatency));
				Assert.AreEqual(expectedLatency, actualLatency);
			}

			private readonly Node node1 = new Node(1.0d, 0.05d);
			private readonly Node node2 = new Node(2.0d, 0.05d);
			private readonly Node node3 = new Node(3.0d, 0.05d);
			private readonly Node node4 = new Node(4.0d, 0.05d);
			private readonly Node node5 = new Node(5.0d, 0.05d);
			private readonly Node node6 = new Node(6.0d, 0.05d);
			private readonly Node node7 = new Node(7.0d, 0.05d);
			private INode parent;
		}
	}
}