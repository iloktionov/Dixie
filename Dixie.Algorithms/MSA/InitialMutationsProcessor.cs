using System;

namespace Dixie.Core
{
	internal partial class InitialMutationsProcessor
	{
		public InitialMutationsProcessor(Int32[] initialSolution, Random random, Double[,] etcMatrix, Double[] availabilityVector)
		{
			this.initialSolution = initialSolution;
			this.random = random;
			this.etcMatrix = etcMatrix;
			completionTimes = MakespanCalculator.GetSortedCompletionTimes(initialSolution, etcMatrix, availabilityVector);
		}

		public SingleExchangeMutation Mutate()
		{
			SingleExchangeMutation mutation = SingleExchangeMutation.Generate(initialSolution, random);
			mutation.Apply(initialSolution);
			return mutation;
		}

		public Double GetMakespan(SingleExchangeMutation mutation)
		{
			Int32 node1 = initialSolution[mutation.FirstIndex];
			Int32 node2 = initialSolution[mutation.SecondIndex];
			completionTimes[node1] += etcMatrix[mutation.FirstIndex, node1];
			completionTimes[node1] -= etcMatrix[mutation.SecondIndex, node1];
			completionTimes[node2] += etcMatrix[mutation.SecondIndex, node2];
			completionTimes[node2] -= etcMatrix[mutation.FirstIndex, node2];
			return ExtendedMath.Max(completionTimes[node1], completionTimes[node2], completionTimes[completionTimes.Length - 1]);
		}

		public Int32[] CloneSolution()
		{
			return initialSolution.CloneSolution();
		}

		public void Rollback(SingleExchangeMutation mutation)
		{
			Int32 node1 = initialSolution[mutation.FirstIndex];
			Int32 node2 = initialSolution[mutation.SecondIndex];
			completionTimes[node1] -= etcMatrix[mutation.FirstIndex, node1];
			completionTimes[node1] += etcMatrix[mutation.SecondIndex, node1];
			completionTimes[node2] -= etcMatrix[mutation.SecondIndex, node2];
			completionTimes[node2] += etcMatrix[mutation.FirstIndex, node2];
			mutation.Apply(initialSolution);
		}

		private readonly Int32[] initialSolution;
		private readonly Double[,] etcMatrix;
		private readonly Double[] completionTimes;
		private readonly Random random;
	}
}