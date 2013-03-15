using System;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class RemoveNodesMutator_Tests
	{
		[Test]
		public void Test_MinRemainingCount()
		{
			Topology topology = GenerateTopology(1000);
			var offlinePool = new OfflineNodesPool();
			var random = new Random();
			const int minRemainingCount = 998;
			var garbageCollector = new GarbageCollector(TimeSpan.Zero);
			var mutator = new RemoveNodesMutator(offlinePool, random, new TopologyConfigurator(), minRemainingCount, garbageCollector);
			mutator.Mutate(topology);
			Assert.AreEqual(minRemainingCount, topology.WorkerNodesCount);
			for (int i = 0; i < 10; i++)
				mutator.Mutate(topology);
			Assert.AreEqual(minRemainingCount, topology.WorkerNodesCount);
		}

		[Test]
		public void Test_CorrectWork()
		{
			Topology topology = GenerateTopology(1000);
			var offlinePool = new OfflineNodesPool();
			var random = new Random();
			var garbageCollector = new GarbageCollector(TimeSpan.Zero);
			var mutator = new RemoveNodesMutator(offlinePool, random, new TopologyConfigurator(),  1, garbageCollector);
			mutator.Mutate(topology);

			Assert.AreEqual(1, topology.WorkerNodesCount);
			foreach (OfflineNodeInfo info in offlinePool.OfflineNodes)
			{
				Assert.True(ReferenceEquals(info.ParentNode, topology.MasterNode));
				switch (info.FailureType)
				{
					case NodeFailureType.ShortTerm: Assert.True(info.OfflineNode.IsComputing()); break;
					case NodeFailureType.LongTerm: Assert.False(info.OfflineNode.IsComputing()); break;
					case NodeFailureType.Permanent: break;
					default: Assert.Fail("Unknown node failure type"); break;
				}
			}

			Console.Out.WriteLine("{0} nodes in GC", garbageCollector.Count);
			if (offlinePool.OfflineNodes.Count < 999)
				Assert.Greater(garbageCollector.Count, 0);
		}

		private Topology GenerateTopology(int nodesCount)
		{
			var topology = Topology.CreateEmpty();
			for (int i = 0; i < nodesCount; i++)
				topology.AddNode(new Node(1, 1), topology.MasterNode, TimeSpan.FromMilliseconds(1));
			return topology;
		}
	}
}