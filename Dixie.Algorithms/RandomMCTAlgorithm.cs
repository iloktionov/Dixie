using System;
using System.Collections.Generic;
using System.Linq;

namespace Dixie.Core
{
	internal class RandomMCTAlgorithm
	{
		public RandomMCTAlgorithm(Random random, int randomAssignations)
		{
			this.random = random;
			this.randomAssignations = randomAssignations;
		}

		public RandomMCTAlgorithm(Random random)
			: this (random, 0) { }

		public Int32[] AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			etcMatrix = ConstructETCMatrix(aliveNodes, pendingTasks);
			Double[] availabilityVector = ConstructAvailabilityVector(aliveNodes);
			var assignations = new Int32[pendingTasks.Count];

			for (int i = 0; i < pendingTasks.Count; i++)
			{
				Int32 assignedNodeIndex;
				if (i < randomAssignations)
					assignedNodeIndex = random.Next(aliveNodes.Count);
				else
				{
					Double minCompletionTime = Double.MaxValue;
					assignedNodeIndex = Int32.MinValue;
					for (int j = 0; j < aliveNodes.Count; j++)
					{
						Double completionTime = availabilityVector[j] + etcMatrix[i, j];
						if (completionTime < minCompletionTime)
						{
							minCompletionTime = completionTime;
							assignedNodeIndex = j;
						}
					}
				}
				availabilityVector[assignedNodeIndex] += etcMatrix[i, assignedNodeIndex];
				assignations[i] = assignedNodeIndex;
			}
			return assignations;
		}

		public Double[,] EtcMatrix
		{
			get { return etcMatrix; }
		}

		// (iloktionov): Элемент в позиции (i, j) соответствует времени выполнения i-го задания j-й машиной.
		protected virtual Double[,] ConstructETCMatrix(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			var etcMatrix = new Double[pendingTasks.Count, aliveNodes.Count];
			for (int i = 0; i < pendingTasks.Count; i++)
				for (int j = 0; j < aliveNodes.Count; j++)
					etcMatrix[i, j] = pendingTasks[i].Volume / aliveNodes[j].Performance;
			return etcMatrix;
		}

		// (iloktionov): Элемент в позиции i соответствует времени, оставшемуся до полной готовности i-й машины.
		protected virtual Double[] ConstructAvailabilityVector(List<NodeInfo> aliveNodes)
		{
			return aliveNodes.Select(info => info.AvailabilityTime.TotalMilliseconds).ToArray();
		}

		private readonly Random random;
		private readonly Int32 randomAssignations;
		private Double[,] etcMatrix;
	}
}