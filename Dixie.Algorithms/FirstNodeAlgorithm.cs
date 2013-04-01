using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Dixie.Core
{
	[Export(typeof(ISchedulerAlgorithm))]
	internal class FirstNodeAlgorithm : ISchedulerAlgorithm
	{
		public FirstNodeAlgorithm(string name)
		{
			Name = name;
		}

		public FirstNodeAlgorithm()
			: this("FirstNodeAlgorithm") { }

		public IEnumerable<TaskAssignation> AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			return pendingTasks.Select(pendingTask => new TaskAssignation(pendingTask, aliveNodes[0].Id));
		}

		public void Reset() { }

		public override string ToString()
		{
			return Name;
		}

		public string Name { get; set; }
	}
}