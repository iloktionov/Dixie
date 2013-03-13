using System;

namespace Dixie.Core
{
	internal class Master
	{
		public Master()
		{
			nodesManager = new NodesManager(TimeSpan.Zero);
			syncObject = new object();
		}

		public HeartBeatResponse HandleHeartBeatMessage(HeartBeatMessage message)
		{
			nodesManager.HandleHeartBeatMessage(message);
			return new HeartBeatResponse(message.NodeId, null);
		}

		private readonly NodesManager nodesManager;
		private readonly object syncObject;
	}
}