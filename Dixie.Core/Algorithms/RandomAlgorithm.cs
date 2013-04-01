using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

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

		public IEnumerable<TaskAssignation> AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			return pendingTasks.Select(pendingTask => new TaskAssignation(pendingTask, aliveNodes[random.Next(aliveNodes.Count)].Id));
		}

		public void Reset()
		{
			random = new Random((int) (random.Next() + DateTime.UtcNow.Ticks));
		}

		public override string ToString()
		{
			return Name;
		}

		public string Name { get; set; }

		private Random random;
	}
}