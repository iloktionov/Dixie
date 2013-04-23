using System;

namespace Dixie.Core
{
	internal partial class CurrentMutationsProcessor
	{
		public CurrentMutationsProcessor(Random random, Double[,] etcMatrix, Double[] availabilityVector)
		{
			this.random = random;
			this.etcMatrix = etcMatrix;
			this.availabilityVector = availabilityVector;
		}

		public void SetCurrentSolution(Int32[] solution)
		{
			currentSolution = solution;
			Double maxCompletionTime;
			completionTimes = MakespanCalculator.GetCompletionTimes(currentSolution, etcMatrix, availabilityVector, out maxCompletionTime, out worstNodeIndex);
		}

		public SingleExchangeMutation Mutate()
		{
			SingleExchangeMutation mutation = SingleExchangeMutation.Generate(currentSolution, random);
			mutation.Apply(currentSolution);
			return mutation;
		}

		public Double GetMakespan(SingleExchangeMutation mutation, out Int32 prevWorstNodeIndex)
		{
			prevWorstNodeIndex = worstNodeIndex;
			Int32 node1 = currentSolution[mutation.FirstIndex];
			Int32 node2 = currentSolution[mutation.SecondIndex];
			completionTimes[node1] += etcMatrix[mutation.FirstIndex, node1];
			completionTimes[node1] -= etcMatrix[mutation.SecondIndex, node1];
			completionTimes[node2] += etcMatrix[mutation.SecondIndex, node2];
			completionTimes[node2] -= etcMatrix[mutation.FirstIndex, node2];

			if (node1 == worstNodeIndex || node2 == worstNodeIndex)
			{
				Double worstTime = Double.MinValue;
				for (int i = 0; i < completionTimes.Length; i++)
				{
					Double completionTime = completionTimes[i];
					if (completionTime > worstTime)
					{
						worstTime = completionTime;
						worstNodeIndex = i;
					}
				}
				return worstTime;
			}
			else
			{
				var indices = new[] { node1, node2, worstNodeIndex };
				Double worstTime = Double.MinValue;
				for (int i = 0; i < 3; i++)
				{
					Double completionTime = completionTimes[indices[i]];
					if (completionTime > worstTime)
					{
						worstTime = completionTime;
						worstNodeIndex = indices[i];
					}
				}
				return worstTime;
			}
		}

		public Int32[] GetSolution()
		{
			return currentSolution;
		}

		public void Rollback(SingleExchangeMutation mutation, Int32 prevWorstNodeIndex)
		{
			Int32 node1 = currentSolution[mutation.FirstIndex];
			Int32 node2 = currentSolution[mutation.SecondIndex];
			completionTimes[node1] -= etcMatrix[mutation.FirstIndex, node1];
			completionTimes[node1] += etcMatrix[mutation.SecondIndex, node1];
			completionTimes[node2] -= etcMatrix[mutation.SecondIndex, node2];
			completionTimes[node2] += etcMatrix[mutation.FirstIndex, node2];
			mutation.Apply(currentSolution);
			worstNodeIndex = prevWorstNodeIndex;
		}

		private readonly Double[,] etcMatrix;
		private readonly Double[] availabilityVector;
		private readonly Random random;

		private Int32[] currentSolution;
		private Double[] completionTimes;
		private Int32 worstNodeIndex;
	}
}