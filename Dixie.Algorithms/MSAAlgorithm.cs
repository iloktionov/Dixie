using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Dixie.Core
{
	[Export(typeof(ISchedulerAlgorithm))]
	internal partial class MSAAlgorithm : ISchedulerAlgorithm
	{
		public MSAAlgorithm(string name, double initialTemperature, double coolingFactor, int iterations)
		{
			Name = name;
			this.initialTemperature = initialTemperature;
			this.coolingFactor = coolingFactor;
			this.iterations = iterations;
			random = new Random();
		}

		public MSAAlgorithm()
			: this("MSAAlgorithm", 1000d, 0.99d, 20 * 1000) { }

		public void Reset() { }

		public IEnumerable<TaskAssignation> AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			pendingTasks.Sort((task, task1) => task1.Volume.CompareTo(task.Volume));
			etcMatrix = MatricesHelper.ConstructETCMatrix(aliveNodes, pendingTasks);
			availabilityVector = MatricesHelper.ConstructAvailabilityVector(aliveNodes);
			initialSolution = new RandomMCTAlgorithm(random, 1).AssignNodes(aliveNodes, pendingTasks);
			if (pendingTasks.Count < aliveNodes.Count / 4)
				return ConvertSolution(initialSolution, aliveNodes, pendingTasks);

			bestSolution = initialSolution;
			bestMakespan = MakespanCalculator.Calculate(initialSolution, etcMatrix, availabilityVector);

			currentSolution = bestSolution;
			currentMakespan = bestMakespan;
			Double temperature = initialTemperature;

			for (int i = 0; i < iterations; i++)
			{
				TryMutateCurrentSolution(temperature);
				TryMutateInitialSolution(temperature);
				temperature *= coolingFactor;
			}
			return ConvertSolution(bestSolution, aliveNodes, pendingTasks);
		}

		public override string ToString()
		{
			return Name;
		}

		public string Name { get; set; }

		private void TryMutateCurrentSolution(Double temperature)
		{
			Int32[] candidate = CloneWithExchange(currentSolution);
			Double makespan = MakespanCalculator.Calculate(candidate, etcMatrix, availabilityVector);
			Double delta = Math.Exp((currentMakespan - makespan) / temperature);
			if (random.NextDouble() < delta)
			{
				currentSolution = candidate;
				currentMakespan = makespan;
				if (currentMakespan < bestMakespan)
				{
					bestMakespan = currentMakespan;
					bestSolution = currentSolution;
				}
			}
		}

		private void TryMutateInitialSolution(Double temperature)
		{
			Int32[] candidate = CloneWithExchange(initialSolution);
			Double makespan = MakespanCalculator.Calculate(candidate, etcMatrix, availabilityVector);
			Double delta = Math.Exp((currentMakespan - makespan) / temperature);
			if (random.NextDouble() < delta)
			{
				currentSolution = candidate;
				currentMakespan = makespan;
				if (currentMakespan < bestMakespan)
				{
					bestMakespan = currentMakespan;
					bestSolution = currentSolution;
				}
			}
		}

		private static IEnumerable<TaskAssignation> ConvertSolution(IEnumerable<Int32> solution, List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			return solution
				.Select((nodeIdx, taskIdx) => new TaskAssignation(pendingTasks[taskIdx], aliveNodes[nodeIdx].Id))
				.ToList();
		} 

		private Int32[] CloneWithExchange(Int32[] solution)
		{
			var newSolution = new Int32[solution.Length];
			Array.Copy(solution, newSolution, solution.Length);
			Int32 index1;
			Int32 index2;
			do
			{
				index1 = random.Next(solution.Length);
				index2 = random.Next(solution.Length);
			}
			while (index1 == index2 || solution[index1] == solution[index2]);
			Int32 tmp = newSolution[index1];
			newSolution[index1] = newSolution[index2];
			newSolution[index2] = tmp; 
			return newSolution;
		}

		private readonly Double initialTemperature;
		private readonly Double coolingFactor;
		private readonly Int32 iterations;
		private readonly Random random;

		private Double[,] etcMatrix;
		private Double[] availabilityVector;
		private Int32[] initialSolution;
		private Int32[] currentSolution;
		private Int32[] bestSolution;
		private Double currentMakespan;
		private Double bestMakespan;
	}
}