using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Dixie.Core
{
	// (iloktionov): Лобовое batch-применение MCT-алгоритма, используемого в online-режиме планирования.
	[Export(typeof(ISchedulerAlgorithm))]
	internal class MCTAlgorithm : ISchedulerAlgorithm
	{
		public MCTAlgorithm(string name)
		{
			Name = name;
		}

		public MCTAlgorithm()
			: this("MCTAlgorithm") { }

		public virtual IEnumerable<TaskAssignation> AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			Double[,] etcMatrix = ConstructETCMatrix(aliveNodes, pendingTasks);
			Double[] availabilityVector = ConstructAvailabilityVector(aliveNodes);
			var assignations = new List<TaskAssignation>(pendingTasks.Count);

			for (int i = 0; i < pendingTasks.Count; i++)
			{
				Double minCompletionTime = Double.MaxValue;
				Int32 assignedNodeIndex = Int32.MinValue;
				for (int j = 0; j < aliveNodes.Count; j++)
				{
					Double completionTime = availabilityVector[j] + etcMatrix[i, j];
					if (completionTime < minCompletionTime)
					{
						minCompletionTime = completionTime;
						assignedNodeIndex = j;
					}
				}
				availabilityVector[assignedNodeIndex] += etcMatrix[i, assignedNodeIndex];
				assignations.Add(new TaskAssignation(pendingTasks[i], aliveNodes[assignedNodeIndex].Id));
			}
			return assignations;
		}

		public virtual void Reset() { }

		public override string ToString()
		{
			return Name;
		}

		public string Name { get; set; }

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
	}
}