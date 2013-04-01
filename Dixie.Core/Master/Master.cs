using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dixie.Core
{
	internal class Master
	{
		public Master(TimeSpan deadabilityThreshold, ILog log)
		{
			this.log = new PrefixedILogWrapper(log, "Master");
			nodesManager = new NodesManager(deadabilityThreshold, this.log);
			taskManager = new TaskManager(this.log);
			watch = Stopwatch.StartNew();
			syncObject = new object();
		}

		internal HeartBeatResponse HandleHeartBeatMessage(HeartBeatMessage message)
		{
			lock (syncObject)
			{
				nodesManager.HandleHeartBeatMessage(message);
				taskManager.ReportTasksCompletion(message.NodeId, message.CompletedTasks);
				return new HeartBeatResponse(message.NodeId, taskManager.GetTasksForNodeOrNull(message.NodeId));
			}
		}

		internal void ExecuteSchedulerAlgorithm(ISchedulerAlgorithm algorithm)
		{
			lock (syncObject)
			{
				List<Guid> deads = nodesManager.RemoveDeadsOrNull();
				taskManager.ReportDeadNodes(deads);

				List<NodeInfo> aliveNodeInfos = nodesManager.GetAliveNodeInfos();
				if (aliveNodeInfos.Count <= 0)
				{
					LogNoAliveNodes();
					return;
				}
				List<Task> pendingTasks = taskManager.GetPendingTasks();
				if (pendingTasks.Count <= 0)
					return;
				watch.Restart();
				TaskAssignation[] assignations = algorithm.AssignNodes(aliveNodeInfos, pendingTasks).ToArray();
				LogAlgorithmWorkTime(watch.Elapsed, algorithm);
				foreach (TaskAssignation assignation in assignations)
					taskManager.AssignNodeToTask(assignation.Task, assignation.Node);
			}
		}

		internal void RefillTasksIfNeeded(TasksGenerator tasksGenerator)
		{
			lock (syncObject)
				if (taskManager.NeedsRefill())
				{
					LogRefillTasks();
					taskManager.PutTasks(tasksGenerator.GenerateTasks());
				}
		}

		internal void CollectGarbage(HashSet<Guid> permanentlyDeletedNodes)
		{
			lock (syncObject)
			{
				watch.Restart();
				nodesManager.CollectGarbage(permanentlyDeletedNodes);
				taskManager.CollectGarbage(permanentlyDeletedNodes);
				LogGarbageCollection(permanentlyDeletedNodes.Count, watch.Elapsed);
			}
		}

		internal Double GetTotalWorkDone()
		{
			lock (syncObject)
				return taskManager.TotalWorkDone;
		}

		internal void DisableAccumulatingResults()
		{
			lock (syncObject)
				taskManager.DisableAccumulatingNewResults();
		}

		public IEnumerable<TaskState> GetTaskStates()
		{
			return taskManager.GetTaskStates();
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

		#region Logging
		private void LogGarbageCollection(int nodesCount, TimeSpan elapsed)
		{
			log.Info("Performed garbage collection of {0} nodes in {1}.", nodesCount, elapsed);
		}

		private void LogRefillTasks()
		{
			log.Info("All tasks are completed. Need to generate new ones..");
		}

		private void LogAlgorithmWorkTime(TimeSpan elapsed, ISchedulerAlgorithm algorithm)
		{
			log.Debug("Executed algorithm {0} in {1}", algorithm.Name, elapsed);
		}

		private void LogNoAliveNodes()
		{
			log.Error("There are no alive nodes for algorithm to operate on.");
		}
		#endregion

		private readonly NodesManager nodesManager;
		private readonly TaskManager taskManager;
		private readonly Stopwatch watch;
		private readonly ILog log;
		private readonly object syncObject;
	}
}