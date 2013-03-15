using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Dixie.Core
{
	internal partial class NodesManager
	{
		[TestFixture]
		internal class NodesManager_Tests
		{
			[Test]
			public void Test_HandleHeartBeatMessage()
			{
				Guid node = Guid.NewGuid();
				var manager = new NodesManager(TimeSpan.Zero);
				manager.HandleHeartBeatMessage(new HeartBeatMessage(node, 1, 1) { CommunicationLatency = TimeSpan.FromSeconds(1)});
				manager.HandleHeartBeatMessage(new HeartBeatMessage(Guid.NewGuid(), 0));
				manager.HandleHeartBeatMessage(new HeartBeatMessage(Guid.NewGuid(), 0));
				Assert.AreEqual(3, manager.AliveNodesCount);
				Assert.AreEqual(1, manager.aliveNodeInfos[node].Performance);
				Assert.AreEqual(1, manager.aliveNodeInfos[node].WorkBufferSize);
				Assert.AreEqual(TimeSpan.FromSeconds(1), manager.aliveNodeInfos[node].CommunicationLatency);
			}

			[Test]
			public void Test_RemoveDeads()
			{
				var manager = new NodesManager(TimeSpan.FromMilliseconds(50));
				manager.HandleHeartBeatMessage(new HeartBeatMessage(Guid.NewGuid(), 0));
				manager.HandleHeartBeatMessage(new HeartBeatMessage(Guid.NewGuid(), 0));
				manager.HandleHeartBeatMessage(new HeartBeatMessage(Guid.NewGuid(), 0));
				Assert.AreEqual(3, manager.AliveNodesCount);
				Assert.Null(manager.RemoveDeadsOrNull());
				Assert.AreEqual(3, manager.AliveNodesCount);
				Thread.Sleep(55);
				Assert.AreEqual(3, manager.RemoveDeadsOrNull().Count);
				Assert.AreEqual(0, manager.AliveNodesCount);
				Assert.AreEqual(3, manager.OfflineNodesCount);
				Assert.AreEqual(3, manager.FailuresCount);
			}

			[Test]
			public void Test_HBMSavesFromOffline()
			{
				var manager = new NodesManager(TimeSpan.FromMilliseconds(5));
				Guid nodeId = Guid.NewGuid();
				for (int i = 0; i < 500; i++)
				{
					Thread.Sleep(1);
					manager.HandleHeartBeatMessage(new HeartBeatMessage(nodeId, 1, 1));
					Assert.Null(manager.RemoveDeadsOrNull());
					Assert.AreEqual(1, manager.AliveNodesCount);
					Assert.AreEqual(0, manager.OfflineNodesCount);
				}
				Assert.AreEqual(0, manager.FailuresCount);
			}

			[Test]
			public void Test_ReturnFromOffline()
			{
				var manager = new NodesManager(TimeSpan.FromMilliseconds(10));
				Guid node1 = Guid.NewGuid();
				Guid node2 = Guid.NewGuid();
				Guid node3 = Guid.NewGuid();
				manager.HandleHeartBeatMessage(new HeartBeatMessage(node1, 1, 1));
				manager.HandleHeartBeatMessage(new HeartBeatMessage(node2, 1, 1));
				manager.HandleHeartBeatMessage(new HeartBeatMessage(node3, 1, 1));
				Thread.Sleep(15);
				manager.HandleHeartBeatMessage(new HeartBeatMessage(node1, 1, 1));
				manager.HandleHeartBeatMessage(new HeartBeatMessage(node3, 1, 1));

				Assert.AreEqual(node2, manager.RemoveDeadsOrNull().Single());
				Assert.AreEqual(2, manager.AliveNodesCount);
				Assert.AreEqual(1, manager.OfflineNodesCount);
				Assert.True(manager.offlineNodeInfos.ContainsKey(node2));

				manager.HandleHeartBeatMessage(new HeartBeatMessage(node2, 100, 200));
				Assert.AreEqual(3, manager.AliveNodesCount);
				Assert.AreEqual(0, manager.OfflineNodesCount);
				Assert.True(manager.aliveNodeInfos.ContainsKey(node2));
				Assert.AreEqual(100, manager.aliveNodeInfos[node2].Performance);
				Assert.AreEqual(200, manager.aliveNodeInfos[node2].WorkBufferSize);
			}

			[Test]
			public void Test_FailureHistory()
			{
				var manager = new NodesManager(TimeSpan.FromMilliseconds(1));
				Guid node = Guid.NewGuid();
				const int FailuresCount = 100;

				for (int i = 0; i < FailuresCount; i++)
				{
					manager.HandleHeartBeatMessage(new HeartBeatMessage(node, i, i));
					Thread.Sleep(2);
					Assert.AreEqual(1, manager.RemoveDeadsOrNull().Count);
				}

				manager.HandleHeartBeatMessage(new HeartBeatMessage(node, 1, 1));
				NodeFailureHistory failureHistory = manager.aliveNodeInfos[node].FailureHistory;
				Assert.True(failureHistory.HasFailures());
				Assert.AreEqual(FailuresCount, failureHistory.Failures.Count);
				Assert.Greater(failureHistory.MinFailureTime(), TimeSpan.FromMilliseconds(1));

				foreach (var VARIABLE in failureHistory.Failures)
				{
					Console.Out.WriteLine(VARIABLE.Duration);
				}
			}
		}
	}
}