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
			return AssignNodesInternal(aliveNodes, pendingTasks).Select((nodeIdx, taskIdx) => new TaskAssignation(pendingTasks[taskIdx], aliveNodes[nodeIdx].Id));
		}

		public Int32[] AssignNodesInternal(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			etcMatrix = ConstructETCMatrix(aliveNodes, pendingTasks);
			availabilityVector = ConstructAvailabilityVector(aliveNodes);
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

		public Double[,] EtcMatrix
		{
			get { return etcMatrix; }
		}

		public Double[] AvailabilityVector
		{
			get { return availabilityVector; }
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

		private Double[,] etcMatrix;
		private Double[] availabilityVector;
	}
}