using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Dixie.Core
{
	// (iloktionov): Opportunistic load balancing algorithm.
	[Export(typeof(ISchedulerAlgorithm))]
	internal class OLBAlgorithm : ISchedulerAlgorithm
	{
		public OLBAlgorithm(string name)
		{
			Name = name;
		}

		public OLBAlgorithm()
			: this("OLBAlgorithm") { }

		public IEnumerable<TaskAssignation> AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			var assignations = new List<TaskAssignation>(pendingTasks.Count);
			var freeNodesPool = new Queue<NodeInfo>(aliveNodes.Where(info => info.AvailabilityTime.Equals(TimeSpan.Zero)));

			for (int i = 0; i < pendingTasks.Count; i++)
			{
				if (freeNodesPool.Count <= 0)
					return assignations;
				assignations.Add(new TaskAssignation(pendingTasks[i], freeNodesPool.Dequeue().Id));
			}
			return assignations;
		}

		public void Reset() { }

		public override string ToString()
		{
			return Name;
		}

		public string Name { get; set; }
	}
}