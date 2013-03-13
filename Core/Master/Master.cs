using System;
using System.Collections.Generic;

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

		public void ExecuteSchedulerAlgorithm(ISchedulerAlgorithm algorithm)
		{
			lock (syncObject)
			{
				List<Guid> deads = nodesManager.RemoveDeadsOrNull();
				taskManager.ReportDeadNodes(deads);
				algorithm.Work(nodesManager.GetAliveNodeInfos(), taskManager);
			}
		}

		internal int AliveNodesCount
		{
			get { return nodesManager.AliveNodesCount; }
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