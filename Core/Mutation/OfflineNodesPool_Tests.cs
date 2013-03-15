using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class OfflineNodesPool_Tests
	{
		[Test]
		public void Test_CorrectWork()
		{
			var pool = new OfflineNodesPool();
			var node1 = new Node(1, 0.1d);
			var node2 = new Node(2, 0.2d);
			pool.Put(node1, null, NodeFailureType.ShortTerm, TimeSpan.Zero);
			pool.Put(node2, null, NodeFailureType.ShortTerm, TimeSpan.FromMilliseconds(25));
			Assert.AreEqual(2, pool.OfflineNodes.Count);
			
			Assert.AreEqual(node1, pool.PopNodesReadyForReturn().Single().OfflineNode);
			Assert.False(pool.PopNodesReadyForReturn().Any());
			Assert.AreEqual(1, pool.OfflineNodes.Count);
			Thread.Sleep(30);
			Assert.AreEqual(node2, pool.PopNodesReadyForReturn().Single().OfflineNode);
			Assert.False(pool.PopNodesReadyForReturn().Any());
			Assert.AreEqual(0, pool.OfflineNodes.Count);
		}
	}
}