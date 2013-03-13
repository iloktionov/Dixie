using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	internal class RandomAlgorithm : ISchedulerAlgorithm
	{
		public RandomAlgorithm(Random random)
		{
			this.random = random;
			Name = String.Format("Random-{0}", random.Next());
		}

		public RandomAlgorithm()
			: this (new Random()) { }

		public void Work(List<NodeInfo> aliveNodes, TaskManager taskManager)
		{
			if (aliveNodes.Count <= 0)
				return;
			foreach (Task pendingTask in taskManager.GetPendingTasks())
				taskManager.AssignNodeToTask(pendingTask, aliveNodes[random.Next(aliveNodes.Count)].Id);
		}

		public string Name { get; private set; }

		private readonly Random random;
	}
}