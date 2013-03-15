using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	internal class Master
	{
		public Master(TimeSpan deadabilityThreshold)
		{
			nodesManager = new NodesManager(deadabilityThreshold);
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

		public void RefillTasksIfNeeded(TasksGenerator tasksGenerator)
		{
			lock (syncObject)
			{
				if (taskManager.NeedsRefill())
					taskManager.PutTasks(tasksGenerator.GenerateTasks());
			}
		}

		public void CollectGarbage(IEnumerable<Guid> permanentlyDeletedNodes)
		{
			lock (syncObject)
			{
				nodesManager.CollectGarbage(permanentlyDeletedNodes);
				taskManager.CollectGarbage(permanentlyDeletedNodes);
			}
		}

		public Double GetTotalWorkDone()
		{
			lock (syncObject)
				return taskManager.TotalWorkDone;
		}

		public void DisableAccumulatingResults()
		{
			lock (syncObject)
				taskManager.DisableAccumulatingNewResults();
		}

		internal int AliveNodesCount
		{
			get { return nodesManager.AliveNodesCount; }
		}

		internal TaskManager TaskManager
		{
			get { return taskManager; }
		}

		internal NodesManager NodesManager
		{
			get { return nodesManager; }
		}

		private readonly NodesManager nodesManager;
		private readonly TaskManager taskManager;
		private readonly object syncObject;
	}
}