using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Dixie.Core
{
	// (iloktionov): Makespan решения, мутирующего из initialSolution можно не считать каждый раз.
	// Достаточно один раз запомнить все completionTimes в отсортированном виде.

	// (iloktionov): Makespan решения, получающегося из current, необходимо полностью пересчитывать лишь тогда, когда перестановка затрагивает худшую ноду.

	[Export(typeof(ISchedulerAlgorithm))]
	internal partial class MSAAlgorithm : ISchedulerAlgorithm
	{
		public MSAAlgorithm(string name, double initialTemperature, double coolingFactor, int iterations, ILog log)
		{
			Name = name;
			this.initialTemperature = initialTemperature;
			this.coolingFactor = coolingFactor;
			this.iterations = iterations;
			this.log = log;
			random = new Random();
		}

		public MSAAlgorithm(ILog log)
			: this("MSAAlgorithm", 1000d, 0.99d, 10 * 1000, log) { }

		public MSAAlgorithm()
			: this(new FakeLog()) { }

		public void Reset() { }

		public IEnumerable<TaskAssignation> AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			etcMatrix = MatricesHelper.ConstructETCMatrix(aliveNodes, pendingTasks);
			availabilityVector = MatricesHelper.ConstructAvailabilityVector(aliveNodes);
			initialSolution = new MCTAlgorithm(etcMatrix).AssignNodesInternal(aliveNodes, pendingTasks);
			if (pendingTasks.Count < aliveNodes.Count / 4)
				return ConvertSolution(initialSolution, aliveNodes, pendingTasks);

			bestSolution = initialSolution;
			bestMakespan = MakespanCalculator.Calculate(initialSolution, etcMatrix, availabilityVector);

			currentSolution = bestSolution;
			currentMakespan = bestMakespan;
			Double temperature = initialTemperature;

			log.Info("Initial solution makespan: {0:0.00000}", bestMakespan);
			for (int i = 0; i < iterations; i++)
			{
				TryMutateCurrentSolution(temperature);
				TryMutateInitialSolution(temperature);
				temperature *= coolingFactor;
			}
			log.Info("Best solution makespan: {0:0.00000}", bestMakespan);

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
					log.Info("Found better makespan: {0:0.00000}", bestMakespan);
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
					log.Info("Found better makespan: {0:0.00000}", bestMakespan);
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
		private readonly ILog log;

		private Double[,] etcMatrix;
		private Double[] availabilityVector;
		private Int32[] initialSolution;
		private Int32[] currentSolution;
		private Int32[] bestSolution;
		private Double currentMakespan;
		private Double bestMakespan;
	}
}