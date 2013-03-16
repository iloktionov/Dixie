using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Dixie.Core
{
	[Export(typeof(ISchedulerAlgorithm))]
	public class RandomAlgorithm : ISchedulerAlgorithm
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

		public void Reset()
		{
			random = new Random((int) (random.Next() + DateTime.UtcNow.Ticks));
		}

		public string Name { get; set; }

		private Random random;
	}
}