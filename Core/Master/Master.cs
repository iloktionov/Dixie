using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dixie.Core
{
	internal class Master
	{
		public Master(TimeSpan deadabilityThreshold, ILog log)
		{
			this.log = new PrefixedILogWrapper(log, "Master");
			nodesManager = new NodesManager(deadabilityThreshold);
			taskManager = new TaskManager();
			watch = Stopwatch.StartNew();
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

				List<NodeInfo> aliveNodeInfos = nodesManager.GetAliveNodeInfos();
				watch.Restart();
				algorithm.Work(aliveNodeInfos, taskManager);
				LogAlgorithmWorkTime(watch.Elapsed, algorithm);
			}
		}

		public void RefillTasksIfNeeded(TasksGenerator tasksGenerator)
		{
			lock (syncObject)
				if (taskManager.NeedsRefill())
				{
					LogRefillTasks();
					taskManager.PutTasks(tasksGenerator.GenerateTasks());
				}
		}

		public void CollectGarbage(List<Guid> permanentlyDeletedNodes)
		{
			lock (syncObject)
			{
				watch.Restart();
				nodesManager.CollectGarbage(permanentlyDeletedNodes);
				taskManager.CollectGarbage(permanentlyDeletedNodes);
				LogGarbageCollection(permanentlyDeletedNodes.Count, watch.Elapsed);
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
			log.Info("Executed algorithm {0} in {1}", algorithm.Name, elapsed);
		}
		#endregion

		private readonly NodesManager nodesManager;
		private readonly TaskManager taskManager;
		private readonly Stopwatch watch;
		private readonly ILog log;
		private readonly object syncObject;
	}
}