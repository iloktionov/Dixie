using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace Dixie.Core
{
	[Export(typeof(ISchedulerAlgorithm))]
	internal class MSAAlgorithm : ISchedulerAlgorithm
	{
		public MSAAlgorithm(string name)
		{
			Name = name;
		}

		public MSAAlgorithm()
			: this("MSAAlgorithm") { }

		public IEnumerable<TaskAssignation> AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			var mctAlgorithm = new MCTAlgorithm();
			Int32[] initialSolution = mctAlgorithm.AssignNodesInternal(aliveNodes, pendingTasks);
			Double[,] etcMatrix = mctAlgorithm.EtcMatrix;
			Double[] availabilityVector = mctAlgorithm.AvailabilityVector;

			throw new NotImplementedException();
		}

		public virtual void Reset() { }

		public override string ToString()
		{
			return Name;
		}

		public string Name { get; set; }
	}
}