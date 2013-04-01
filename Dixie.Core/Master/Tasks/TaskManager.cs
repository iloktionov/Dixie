using System;
using System.Collections.Generic;
using System.Linq;

namespace Dixie.Core
{
	internal partial class TaskManager
	{
		internal TaskManager(ILog log)
		{
			this.log = new PrefixedILogWrapper(log, "TaskManager");
			taskStates = new Dictionary<Guid, TaskState>();
			assignationsMap = new Dictionary<Guid, List<Task>>();
			completedTasksCount = 0;
			TotalWorkDone = 0;
			accumulateNewResults = true;
		}

		internal void PutTasks(IEnumerable<Task> tasks)
		{
			taskStates = tasks.ToDictionary(task => task.Id, task => new TaskState(task));
			assignationsMap.Clear();
			completedTasksCount = 0;
		}

		internal bool NeedsRefill()
		{
			return completedTasksCount >= taskStates.Count;
		}

		internal List<Task> GetPendingTasks()
		{
			return taskStates.Values
				.Where(state => state.Status == TaskStatus.Pending)
				.Select(state => state.Task)
				.ToList();
		}

		internal void AssignNodeToTask(Task task, Guid nodeId)
		{
			taskStates[task.Id].AssignNode(nodeId);
			List<Task> nodeAssignations;
			if (assignationsMap.TryGetValue(nodeId, out nodeAssignations))
				nodeAssignations.Add(task);
			else
			{
				nodeAssignations = new List<Task>{task};
				assignationsMap.Add(nodeId, nodeAssignations);
			}
		}

		internal Double TotalWorkDone { get; private set; }

		internal void ReportDeadNodes(List<Guid> deads)
		{
			if (deads == null)
				return;
			foreach (Guid deadNode in deads)
				foreach (KeyValuePair<Guid, TaskState> pair in taskStates)
					pair.Value.ReportNodeFailure(deadNode);
		}

		internal void ReportTasksCompletion(Guid nodeId, List<Guid> completedTasks)
		{
			if (completedTasks == null)
				return;
			// (iloktionov): Когда приходят результаты, задания вполне уже может и не оказаться здесь. 
			// А даже если задание присутствует, оно может оказаться уже выполненным другой нодой.
			foreach (Guid completedTask in completedTasks)
			{
				TaskState state;
				if (taskStates.TryGetValue(completedTask, out state) && state.ReportCompletion(nodeId))
				{
					completedTasksCount++;
					if (accumulateNewResults)
						TotalWorkDone += state.Task.Volume;
				}
				LogCompletionProgress();
			}
		}

		internal List<Task> GetTasksForNodeOrNull(Guid nodeId)
		{
			List<Task> nodeAssignations;
			if (!assignationsMap.TryGetValue(nodeId, out nodeAssignations))
				return null;
			assignationsMap.Remove(nodeId);
			return nodeAssignations;
		}

		internal void DisableAccumulatingNewResults()
		{
			accumulateNewResults = false;
		}

		internal void CollectGarbage(IEnumerable<Guid> permanentlyDeletedNodes)
		{
			foreach (Guid nodeId in permanentlyDeletedNodes)
				assignationsMap.Remove(nodeId);
		}

		internal IEnumerable<TaskState> GetTaskStates()
		{
			return taskStates.Values;
		} 

		#region Logging
		private void LogCompletionProgress()
		{
			log.Debug("Completed tasks: {0}/{1}.", completedTasksCount, taskStates.Count);
		}
		#endregion

		private readonly Dictionary<Guid, List<Task>> assignationsMap;
		private readonly ILog log;
		private Dictionary<Guid, TaskState> taskStates;
		private int completedTasksCount;
		private volatile bool accumulateNewResults;
	}
}