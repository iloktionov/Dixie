using System;

namespace Dixie.Core
{
	public class TaskAssignation
	{
		public TaskAssignation(Task task, Guid node)
		{
			Task = task;
			Node = node;
		}

		public Task Task { get; private set; }
		public Guid Node { get; private set; }
	}
}