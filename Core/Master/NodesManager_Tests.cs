using System;
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
				Assert.AreEqual(3, manager.aliveNodeInfos.Count);
				Assert.AreEqual(1, manager.aliveNodeInfos[node].Performance);
				Assert.AreEqual(1, manager.aliveNodeInfos[node].WorkBufferSize);
				Assert.AreEqual(TimeSpan.FromSeconds(1), manager.aliveNodeInfos[node].CommunicationLatency);
			}

			[Test]
			public void Test_UpdateState()
			{
				var manager = new NodesManager(TimeSpan.FromMilliseconds(50));
				manager.HandleHeartBeatMessage(new HeartBeatMessage(Guid.NewGuid(), 0));
				manager.HandleHeartBeatMessage(new HeartBeatMessage(Guid.NewGuid(), 0));
				manager.HandleHeartBeatMessage(new HeartBeatMessage(Guid.NewGuid(), 0));
				Assert.AreEqual(3, manager.aliveNodeInfos.Count);
				manager.UpdateState();
				Assert.AreEqual(3, manager.aliveNodeInfos.Count);
				Thread.Sleep(55);
				manager.UpdateState();
				Assert.AreEqual(0, manager.aliveNodeInfos.Count);
				Assert.AreEqual(3, manager.hbmTimestamps.Count);
			}
		}
	}
}