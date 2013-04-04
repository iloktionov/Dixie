﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Dixie.Core
{
	internal abstract class MinAlgorithmBase : ISchedulerAlgorithm
	{
		protected MinAlgorithmBase(string name)
		{
			Name = name;
		}

		protected abstract Double GetInitialBestCompletionTime();
		protected abstract bool IsBetterTime(Double completionTime, Double currentBestTime);

		public IEnumerable<TaskAssignation> AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			Double[][] etcMatrix = ConstructETCMatrix(aliveNodes, pendingTasks);
			Double[][] ctMatrix = ConstructCTMatrix(aliveNodes, pendingTasks, etcMatrix);
			var assignations = new List<TaskAssignation>(pendingTasks.Count);

			var metaTask = new Dictionary<Guid, int>(pendingTasks.Count);
			for (int i = 0; i < pendingTasks.Count; i++)
				metaTask.Add(pendingTasks[i].Id, i);

			while (metaTask.Any())
			{
				Double bestCompletionTime = GetInitialBestCompletionTime();
				Int32 assignedIndex = Int32.MinValue;
				Int32 taskIndex = 0;
				foreach (KeyValuePair<Guid, int> pair in metaTask)
				{
					Double minCompletionTime = Double.MaxValue;
					Int32 assignedNodeIndex = Int32.MinValue;
					for (int i = 0; i < aliveNodes.Count; i++)
					{
						Double completionTime = ctMatrix[pair.Value][i];
						if (completionTime < minCompletionTime)
						{
							minCompletionTime = completionTime;
							assignedNodeIndex = i;
						}
					}
					if (IsBetterTime(minCompletionTime, bestCompletionTime))
					{
						bestCompletionTime = minCompletionTime;
						assignedIndex = assignedNodeIndex;
						taskIndex = pair.Value;
					}
				}
				assignations.Add(new TaskAssignation(pendingTasks[taskIndex], aliveNodes[assignedIndex].Id));
				metaTask.Remove(pendingTasks[taskIndex].Id);

				// (iloktionov): Теперь нужно обновить CT-матрицу для оставшихся заданий.
				Double executionTime = etcMatrix[taskIndex][assignedIndex];
				foreach (KeyValuePair<Guid, int> pair in metaTask)
					ctMatrix[pair.Value][assignedIndex] += executionTime;
			}

			return assignations;
		}

		public void Reset() { }

		public override string ToString()
		{
			return Name;
		}

		public string Name { get; set; }

		// (iloktionov): Элемент в позиции (i, j) соответствует времени выполнения i-го задания j-й машиной.
		private static Double[][] ConstructETCMatrix(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			var etcMatrix = new Double[pendingTasks.Count][];
			for (int i = 0; i < pendingTasks.Count; i++)
			{
				etcMatrix[i] = new Double[aliveNodes.Count];
				for (int j = 0; j < aliveNodes.Count; j++)
					etcMatrix[i][j] = pendingTasks[i].Volume / aliveNodes[j].Performance;
			}
			return etcMatrix;
		}

		// (iloktionov): Элемент в позиции (i, j) соответствует времени освобождения j-й машины при условии, что ей назначат i-е задание.
		private static Double[][] ConstructCTMatrix(List<NodeInfo> aliveNodes, List<Task> pendingTasks, Double[][] etcMatrix)
		{
			var ctMatrix = new Double[pendingTasks.Count][];
			for (int i = 0; i < pendingTasks.Count; i++)
			{
				ctMatrix[i] = new Double[aliveNodes.Count];
				for (int j = 0; j < aliveNodes.Count; j++)
					ctMatrix[i][j] = aliveNodes[j].AvailabilityTime.TotalMilliseconds + etcMatrix[i][j];
			}
			return ctMatrix;
		}
	}
}