using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	internal class RandomAlgorithm : ISchedulerAlgorithm
	{
		public RandomAlgorithm(Random random, string name)
		{
			this.random = random;
			Name = name;
		}

		public RandomAlgorithm(Random random)
			: this (random, "Random") { }

		public RandomAlgorithm(string name)
			: this (new Random(), name) { }

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