using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Dixie.Core
{
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
			: this("MSAAlgorithm", 1000d, 0.99d, 200 * 1000, log) { }

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

			bestSolution = initialSolution.CloneSolution();
			bestMakespan = MakespanCalculator.Calculate(initialSolution, etcMatrix, availabilityVector);

			currentMakespan = bestMakespan;
			Double temperature = initialTemperature;

			var initialProcessor = new InitialMutationsProcessor(initialSolution, random, etcMatrix, availabilityVector);
			var currentProcessor = new CurrentMutationsProcessor(random, etcMatrix, availabilityVector);
			currentProcessor.SetCurrentSolution(initialSolution.CloneSolution());
			log.Info("Initial solution makespan: {0:0.00000}", bestMakespan);
			for (int i = 0; i < iterations; i++)
			{
				TryMutateCurrentSolution(temperature, currentProcessor);
				TryMutateInitialSolution(temperature, initialProcessor, currentProcessor);
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

		private void TryMutateCurrentSolution(Double temperature, CurrentMutationsProcessor currentProcessor)
		{
			SingleExchangeMutation mutation = currentProcessor.Mutate();
			int prevWorstNodeIndex;
			Double makespan = currentProcessor.GetMakespan(mutation, out prevWorstNodeIndex);
			Double delta = Math.Exp((currentMakespan - makespan) / temperature);
			if (random.NextDouble() < delta)
				currentMakespan = makespan;
			else currentProcessor.Rollback(mutation, prevWorstNodeIndex);
			if (currentMakespan < bestMakespan)
			{
				bestMakespan = currentMakespan;
				bestSolution = currentProcessor.GetSolution().CloneSolution();
				log.Info("Found better makespan: {0:0.00000}", bestMakespan);
			}
		}

		private void TryMutateInitialSolution(Double temperature, InitialMutationsProcessor initialProcessor, CurrentMutationsProcessor currentProcessor)
		{
			SingleExchangeMutation mutation = initialProcessor.Mutate();
			Double makespan = initialProcessor.GetMakespan(mutation);
			Double delta = Math.Exp((currentMakespan - makespan) / temperature);
			if (random.NextDouble() < delta)
			{
				Int32[] currentSolution = initialProcessor.CloneSolution();
				currentProcessor.SetCurrentSolution(currentSolution);
				currentMakespan = makespan;
				if (currentMakespan < bestMakespan)
				{
					bestMakespan = currentMakespan;
					bestSolution = currentSolution.CloneSolution();
					log.Info("Found better makespan: {0:0.00000}", bestMakespan);
				}
			}
			initialProcessor.Rollback(mutation);
		}

		private static IEnumerable<TaskAssignation> ConvertSolution(IEnumerable<Int32> solution, List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			return solution
				.Select((nodeIdx, taskIdx) => new TaskAssignation(pendingTasks[taskIdx], aliveNodes[nodeIdx].Id))
				.ToList();
		} 

		private readonly Double initialTemperature;
		private readonly Double coolingFactor;
		private readonly Int32 iterations;
		private readonly Random random;
		private readonly ILog log;

		private Double[,] etcMatrix;
		private Double[] availabilityVector;
		private Int32[] initialSolution;
		private Int32[] bestSolution;
		private Double currentMakespan;
		private Double bestMakespan;
	}
}