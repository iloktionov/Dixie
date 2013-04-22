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

		public IEnumerable<TaskAssignation> AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			Double[,] etcMatrix = MatricesHelper.ConstructETCMatrix(aliveNodes, pendingTasks);
			Double[] availabilityVector = MatricesHelper.ConstructAvailabilityVector(aliveNodes);
			Int32[] initialSolution = new MCTAlgorithm(etcMatrix).AssignNodesInternal(aliveNodes, pendingTasks);
			if (pendingTasks.Count < aliveNodes.Count / 4)
				return ConvertSolution(initialSolution, aliveNodes, pendingTasks);

			Int32[] bestSolution = initialSolution;
			Double bestMakespan = MakespanCalculator.Calculate(initialSolution, etcMatrix, availabilityVector);

			Int32[] currentSolution = bestSolution;
			Double currentMakespan = bestMakespan;
			Double temperature = initialTemperature;

			log.Info("Initial solution makespan: {0:0.00000}", bestMakespan);
			for (int i = 0; i < iterations; i++)
			{
				Int32[] solution1 = CloneWithExchange(currentSolution);
				Int32[] solution2 = CloneWithExchange(initialSolution);
				var solutions = new[] {solution1, solution2};
				for (int j = 0; j < 2; j++)
				{
					Double makespan = MakespanCalculator.Calculate(solutions[j], etcMatrix, availabilityVector);
					Double delta = Math.Exp((currentMakespan - makespan) / temperature);
					if (random.NextDouble() < delta)
					{
						currentSolution = solutions[j];
						currentMakespan = makespan;
					}
					if (currentMakespan < bestMakespan)
					{
						bestMakespan = currentMakespan;
						bestSolution = currentSolution;
						log.Info("Found better makespan: {0:0.00000}", bestMakespan);
					}
				}
				temperature *= coolingFactor;
			}
			log.Info("Best solution makespan: {0:0.00000}", bestMakespan);

			return ConvertSolution(bestSolution, aliveNodes, pendingTasks);
		}

		public virtual void Reset() { }

		public override string ToString()
		{
			return Name;
		}

		public string Name { get; set; }

		private static IEnumerable<TaskAssignation> ConvertSolution(IEnumerable<Int32> solution, List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			return solution
				.Select((nodeIdx, taskIdx) => new TaskAssignation(pendingTasks[taskIdx], aliveNodes[nodeIdx].Id))
				.ToList();
		} 

		private Int32[] CloneWithExchange(Int32[] initialSolution)
		{
			var newSolution = new Int32[initialSolution.Length];
			for (int i = 0; i < initialSolution.Length; i++)
				newSolution[i] = initialSolution[i];
			SingleExchangeMutation mutation = SingleExchangeMutation.Generate(initialSolution, random);
			mutation.Apply(newSolution);
			return newSolution;
		}

		private readonly Double initialTemperature;
		private readonly Double coolingFactor;
		private readonly Int32 iterations;
		private readonly Random random;
		private readonly ILog log;
	}
}