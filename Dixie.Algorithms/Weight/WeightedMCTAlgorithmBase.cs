using System;
using System.Collections.Generic;
using System.Linq;

namespace Dixie.Core
{
	internal class WeightedMCTAlgorithmBase : MCTAlgorithm
	{
		public WeightedMCTAlgorithmBase(string name, IWeightSelector weightSelector) 
			: base(name)
		{
			this.weightSelector = weightSelector;
		}

		public override void Reset()
		{
			base.Reset();
			weightSelector.Reset();
		}

		public override IEnumerable<TaskAssignation> AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			weightSelector.Update(aliveNodes);
			return base.AssignNodes(aliveNodes, pendingTasks);
		}

		protected override Double[,] ConstructETCMatrix(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			var etcMatrix = new Double[pendingTasks.Count, aliveNodes.Count];
			for (int i = 0; i < pendingTasks.Count; i++)
				for (int j = 0; j < aliveNodes.Count; j++)
					etcMatrix[i, j] = (pendingTasks[i].Volume / aliveNodes[j].Performance) * weightSelector.GetWeight(aliveNodes[j].Id);
			return etcMatrix;
		}

		// (iloktionov): Элемент в позиции i соответствует времени, оставшемуся до полной готовности i-й машины.
		protected override Double[] ConstructAvailabilityVector(List<NodeInfo> aliveNodes)
		{
			return aliveNodes.Select(info => info.AvailabilityTime.TotalMilliseconds * weightSelector.GetWeight(info.Id)).ToArray();
		}

		private readonly IWeightSelector weightSelector;
	}
}