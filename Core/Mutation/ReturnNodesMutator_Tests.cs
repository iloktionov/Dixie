using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class ReturnNodesMutator_Tests
	{
		[Test]
		public void Test_ReturnNodes()
		{
			var topology = Topology.CreateEmpty();
			var offlinePool = new OfflineNodesPool();
			var configurator = new TopologyConfigurator();
			var mutator = new ReturnNodesMutator(offlinePool, configurator);

			var nodes = new List<Node>();
			for (int i = 0; i < 1000; i++)
			{
				var node = new Node(1, 0);
				node.StopComputing();
				nodes.Add(node);
				offlinePool.Put(node, topology.MasterNode, NodeFailureType.LongTerm, TimeSpan.Zero);
			}

			mutator.Mutate(topology);
			Assert.AreEqual(1000, topology.WorkerNodesCount);
			Assert.AreEqual(1000, topology.Graph.EdgeCount);
			Node n;
			foreach (Node node in nodes)
			{
				Assert.True(node.IsComputing());
				Assert.True(topology.TryGetNode(node.Id, out n));
				Assert.True(ReferenceEquals(node, n));
			}
		}
	}
}