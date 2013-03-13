using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	[Serializable]
	public class ComparisonTestResult
	{
		public ComparisonTestResult()
		{
			AlgorithmResults = new Dictionary<string, AlgorithmTestResult>();
		}

		public string GetWinnerOrNull()
		{
			return AlgorithmResults.Count > 0
				? AlgorithmResults.MaxBy(pair => pair.Value.TotalWorkDone).Key
				: null;
		}

		internal void AddAlgorithmResult(ISchedulerAlgorithm algorithm, AlgorithmTestResult result)
		{
			AlgorithmResults[algorithm.Name] = result;
		}

		public Dictionary<string, AlgorithmTestResult> AlgorithmResults { get; private set; } 
	}
}