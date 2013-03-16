using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Dixie.Core
{
	[Export(typeof(ISchedulerAlgorithm))]
	internal class RandomAlgorithm : ISchedulerAlgorithm
	{
		public RandomAlgorithm(Random random, string name)
		{
			this.random = random;
			Name = name;
		}

		public RandomAlgorithm(Random random)
			: this (random, "RandomAlgorithm") { }

		public RandomAlgorithm(string name)
			: this (new Random(), name) { }

		public RandomAlgorithm() 
			: this (new Random()) { }

		public void Work(List<NodeInfo> aliveNodes, TaskManager taskManager)
		{
			if (aliveNodes.Count <= 0)
				return;
			foreach (Task pendingTask in taskManager.GetPendingTasks())
				taskManager.AssignNodeToTask(pendingTask, aliveNodes[random.Next(aliveNodes.Count)].Id);
		}

		public void Reset() { }

		public string Name { get; set; }

		private readonly Random random;
	}
}