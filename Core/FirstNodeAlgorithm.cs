using System.Collections.Generic;

namespace Dixie.Core
{
	internal class FirstNodeAlgorithm : ISchedulerAlgorithm
	{
		public FirstNodeAlgorithm()
		{
			Name = "FirstNodeAlgorithm";
		}

		public void Work(List<NodeInfo> aliveNodes, TaskManager taskManager)
		{
			if (aliveNodes.Count <= 0)
				return;
			foreach (Task pendingTask in taskManager.GetPendingTasks())
				taskManager.AssignNodeToTask(pendingTask, aliveNodes[0].Id);
		}

		public string Name { get; private set; }
	}
}