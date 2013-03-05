using System;
using System.IO;
using System.Linq;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class Topology_Tests
	{
		[Test]
		public void Test_AddNode()
		{
			Topology topology = Topology.CreateEmpty();
			Assert.Throws<ArgumentNullException>(() => topology.AddNode(null, null, TimeSpan.FromMilliseconds(1)));
			Assert.Throws<ArgumentOutOfRangeException>(() => topology.AddNode(new Node(0, 0), null, TimeSpan.FromMilliseconds(1).Negate()));
			// Добавим новую ноду к мастеру.
			topology.AddNode(node1, null, TimeSpan.FromMilliseconds(1));
			// Нельзя добавить одну ноду дважды.
			Assert.Throws<InvalidOperationException>(() => topology.AddNode(node1, null, TimeSpan.FromMilliseconds(1)));
			// Нельзя добавить ноду к несуществующему родителю.
			Assert.Throws<InvalidOperationException>(() => topology.AddNode(node2, node3, TimeSpan.FromMilliseconds(1)));
			// Но можно к существующему (не мастеру).
			topology.AddNode(node2, node1, TimeSpan.FromMilliseconds(1));
			topology.AddNode(node3, node1, TimeSpan.FromMilliseconds(1));
		}

		[Test]
		public void Test_Serialization()
		{
			Topology topology = Topology.CreateEmpty();
			topology.AddNode(node1, null, TimeSpan.FromMilliseconds(1));
			topology.AddNode(node2, null, TimeSpan.FromMilliseconds(1));
			topology.AddNode(node3, null, TimeSpan.FromMilliseconds(1));

			var stream = new MemoryStream();
			topology.Serialize(stream);
			stream.Seek(0, SeekOrigin.Begin);
			Topology deserializedTopology = Topology.Deserialize(stream);
			Assert.True(deserializedTopology.GetWorkerNodes().SequenceEqual(topology.GetWorkerNodes()));
		}

		private readonly Node node1 = new Node(1.0d, 0.05d);
		private readonly Node node2 = new Node(2.0d, 0.05d);
		private readonly Node node3 = new Node(3.0d, 0.05d);
		private readonly Node node4 = new Node(4.0d, 0.05d);
		private readonly Node node5 = new Node(5.0d, 0.05d);
	}
}