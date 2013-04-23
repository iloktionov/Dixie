using System;

namespace Dixie.Core
{
	internal static class MakespanCalculator
	{
		public static Double Calculate(Int32[] solution, Double[,] etcMatrix, Double[] availabilityVector)
		{
			if (completionTimes == null || completionTimes.Length < availabilityVector.Length)
				completionTimes = new Double[availabilityVector.Length];
			Double maxCompletionTime = 0d;
			for (int i = 0; i < availabilityVector.Length; i++)
			{
				completionTimes[i] = availabilityVector[i];
				if (completionTimes[i] > maxCompletionTime)
					maxCompletionTime = completionTimes[i];
			}
			for (int i = 0; i < solution.Length; i++)
			{
				Int32 nodeIndex = solution[i];
				completionTimes[nodeIndex] += etcMatrix[i, nodeIndex];
				if (completionTimes[nodeIndex] > maxCompletionTime)
					maxCompletionTime = completionTimes[nodeIndex];
			}
			return maxCompletionTime;
		}

		private static Double[] completionTimes;
	}
}