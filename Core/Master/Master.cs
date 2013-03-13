namespace Dixie.Core
{
	internal class Master
	{
		public Master()
		{
			nodesManager = new NodesManager();
			syncObject = new object();
		}

		public HeartBeatResponse HandleHeartBeatMessage(HeartBeatMessage message)
		{
			return null;
		}

		private readonly NodesManager nodesManager;
		private readonly object syncObject;
	}
}