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

		protected override double[][] ConstructCTMatrix(List<NodeInfo> aliveNodes, List<Task> pendingTasks, double[][] etcMatrix)
		{
			var ctMatrix = new Double[pendingTasks.Count][];
			for (int i = 0; i < pendingTasks.Count; i++)
			{
				ctMatrix[i] = new Double[aliveNodes.Count];
				for (int j = 0; j < aliveNodes.Count; j++)
					ctMatrix[i][j] = aliveNodes[j].AvailabilityTime.TotalMilliseconds * weightSelector.GetWeight(aliveNodes[j].Id) + etcMatrix[i][j];
			}
			return ctMatrix;
		}

		private readonly IWeightSelector weightSelector;
	}
}