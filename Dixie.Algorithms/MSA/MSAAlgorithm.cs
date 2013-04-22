using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;

namespace Dixie.Core
{
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
			var mctAlgorithm = new MCTAlgorithm();
			Int32[] initialSolution = mctAlgorithm.AssignNodesInternal(aliveNodes, pendingTasks);
			Double[,] etcMatrix = ConstructETCMatrix(aliveNodes, pendingTasks);
			Double[] availabilityVector = ConstructAvailabilityVector(aliveNodes);

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

			return bestSolution
				.Select((nodeIdx, taskIdx) => new TaskAssignation(pendingTasks[taskIdx], aliveNodes[nodeIdx].Id))
				.ToList();
		}

		public virtual void Reset() { }

		public override string ToString()
		{
			return Name;
		}

		public string Name { get; set; }

		private Int32[] CloneWithExchange(Int32[] initialSolution)
		{
			var newSolution = new Int32[initialSolution.Length];
			for (int i = 0; i < initialSolution.Length; i++)
				newSolution[i] = initialSolution[i];
			ApplySingleExchangeMutation(ref newSolution);
			return newSolution;
		}

		private void ApplySingleExchangeMutation(ref Int32[] solution)
		{
			Int32 index1;
			Int32 index2;
			do
			{
				index1 = random.Next(solution.Length);
				index2 = random.Next(solution.Length);
			}
			while (index1 == index2 || solution[index1] == solution[index2]);
			Int32 tmp = solution[index1];
			solution[index1] = solution[index2];
			solution[index2] = tmp;
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
		private static Double[] ConstructAvailabilityVector(IEnumerable<NodeInfo> aliveNodes)
		{
			return aliveNodes.Select(info => info.AvailabilityTime.TotalMilliseconds).ToArray();
		}

		private readonly Double initialTemperature;
		private readonly Double coolingFactor;
		private readonly Int32 iterations;
		private readonly Random random;
		private readonly ILog log;
	}
}