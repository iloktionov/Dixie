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

		internal MCTAlgorithm(Double[,] etcMatrix)
		{
			this.etcMatrix = etcMatrix;
		}

		public virtual IEnumerable<TaskAssignation> AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			return AssignNodesInternal(aliveNodes, pendingTasks)
				.Select((nodeIdx, taskIdx) => new TaskAssignation(pendingTasks[taskIdx], aliveNodes[nodeIdx].Id))
				.ToList();
		}

		public Int32[] AssignNodesInternal(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			if (etcMatrix == null)
				etcMatrix = MatricesHelper.ConstructETCMatrix(aliveNodes, pendingTasks);
			Double[] availabilityVector = MatricesHelper.ConstructAvailabilityVector(aliveNodes);
			var assignations = new Int32[pendingTasks.Count];

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
				assignations[i] = assignedNodeIndex;
			}
			return assignations;
		}

		public virtual void Reset() { }

		public override string ToString()
		{
			return Name;
		}

		public string Name { get; set; }

		private Double[,] etcMatrix;
	}
}