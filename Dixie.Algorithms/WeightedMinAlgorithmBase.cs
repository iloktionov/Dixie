using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	internal abstract class WeightedMinAlgorithmBase : MinAlgorithmBase
	{
		protected WeightedMinAlgorithmBase(string name, IWeightSelector weightSelector) 
			: base(name)
		{
			this.weightSelector = weightSelector;
		}

		public override void Reset()
		{
			weightSelector.Reset();
			base.Reset();
		}

		public override IEnumerable<TaskAssignation> AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			weightSelector.Update(aliveNodes);
			return base.AssignNodes(aliveNodes, pendingTasks);
		}

		protected override double[][] ConstructETCMatrix(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			var etcMatrix = new Double[pendingTasks.Count][];
			for (int i = 0; i < pendingTasks.Count; i++)
			{
				etcMatrix[i] = new Double[aliveNodes.Count];
				for (int j = 0; j < aliveNodes.Count; j++)
					etcMatrix[i][j] = (pendingTasks[i].Volume / aliveNodes[j].Performance) * weightSelector.GetWeight(aliveNodes[j].Id);
			}
			return etcMatrix;
		}

		private readonly IWeightSelector weightSelector;
	}
}