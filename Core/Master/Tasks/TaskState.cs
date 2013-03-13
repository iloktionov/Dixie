using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	public class TaskState
	{
		public TaskState(Task task)
		{
			Task = task;
			Status = TaskStatus.Pending;
			AssignedNodes = new List<Guid>();
		}

		public void AssignNode(Guid nodeId)
		{
			if (Status == TaskStatus.Completed)
				throw new InvalidOperationException("Can't assign node to a completed task..");
			if (AssignedNodes.Contains(nodeId))
				throw new InvalidOperationException(String.Format("Node {0} is already assigned to this task.", nodeId));
			AssignedNodes.Add(nodeId);
			Status = TaskStatus.Assigned;
		}

		public void ReportNodeFailure(Guid nodeId)
		{
			if (Status == TaskStatus.Completed)
				return;
			AssignedNodes.Remove(nodeId);
			if (AssignedNodes.Count <= 0)
				Status = TaskStatus.Pending;
		}

		public void ReportCompletion(Guid nodeId)
		{
			Status = TaskStatus.Completed;
		}

		public Task Task { get; private set; }
		public TaskStatus Status { get; private set; }
		internal List<Guid> AssignedNodes { get; private set; }
	}
}