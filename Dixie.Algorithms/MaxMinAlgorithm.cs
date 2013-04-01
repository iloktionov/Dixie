using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Dixie.Core
{
	[Export(typeof(ISchedulerAlgorithm))]
	internal class MaxMinAlgorithm : MCTAlgorithm
	{
		public MaxMinAlgorithm(string name)
			: base(name) { }

		public MaxMinAlgorithm()
			: base("MaxMinAlgorithm") { }

		protected override void PrepareTasks(List<Task> tasks)
		{
			tasks.Sort((task1, task2) => task2.Volume.CompareTo(task1.Volume));
		}
	}
}