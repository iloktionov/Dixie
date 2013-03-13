using System;

namespace Dixie.Core
{
	internal class Master
	{
		public Master()
		{
			nodesManager = new NodesManager(TimeSpan.Zero);
			taskManager = new TaskManager();
			syncObject = new object();
		}

		public HeartBeatResponse HandleHeartBeatMessage(HeartBeatMessage message)
		{
			lock (syncObject)
			{
				nodesManager.HandleHeartBeatMessage(message);
				taskManager.ReportTasksCompletion(message.NodeId, message.CompletedTasks);
				return new HeartBeatResponse(message.NodeId, taskManager.GetTasksForNodeOrNull(message.NodeId));
			}
		}

		internal TaskManager TaskManager
		{
			get { return taskManager; }
		}

		private readonly NodesManager nodesManager;
		private readonly TaskManager taskManager;
		private readonly object syncObject;
	}
}