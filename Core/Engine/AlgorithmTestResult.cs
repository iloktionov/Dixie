using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	[Serializable]
	internal class AlgorithmTestResult
	{
		public AlgorithmTestResult()
		{
			IntermediateResults = new List<IntermediateTestResult>();
			TotalWorkDone = 0;
		}

		public void AddIntermediateResult(Double workDone, TimeSpan timeElapsed)
		{
			IntermediateResults.Add(new IntermediateTestResult(workDone, timeElapsed));
			TotalWorkDone = workDone; 
		}

		public double TotalWorkDone { get; private set; }
		public List<IntermediateTestResult> IntermediateResults { get; private set; }
	}
}