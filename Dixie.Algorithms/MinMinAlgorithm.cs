using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Dixie.Core
{
	[Export(typeof(ISchedulerAlgorithm))]
	internal class MinMinAlgorithm : MCTAlgorithm
	{
		public MinMinAlgorithm(string name) 
			: base(name) { }

		public MinMinAlgorithm()
			: base("MinMinAlgorithm") { }

		protected override void PrepareTasks(List<Task> tasks)
		{
			tasks.Sort((task1, task2) => task1.Volume.CompareTo(task2.Volume));
		}		
	}
}