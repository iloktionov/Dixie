using System.Collections.Generic;
using System.ComponentModel.Composition;

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

		public void Work(List<NodeInfo> aliveNodes, TaskManager taskManager)
		{
			if (aliveNodes.Count <= 0)
				return;
			foreach (Task pendingTask in taskManager.GetPendingTasks())
				taskManager.AssignNodeToTask(pendingTask, aliveNodes[0].Id);
		}

		public void Reset() { }

		public override string ToString()
		{
			return Name;
		}

		public string Name { get; set; }
	}
}