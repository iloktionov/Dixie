using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	internal class RandomAlgorithm : ISchedulerAlgorithm
	{
		public RandomAlgorithm(Random random)
		{
			this.random = random;
		}

		public RandomAlgorithm()
			: this (new Random()) { }

		public void Work(List<NodeInfo> aliveNodes, TaskManager taskManager)
		{
			foreach (Task pendingTask in taskManager.GetPendingTasks())
				taskManager.AssignNodeToTask(pendingTask, aliveNodes[random.Next(aliveNodes.Count)].Id);
		}

		private readonly Random random;
	}
}