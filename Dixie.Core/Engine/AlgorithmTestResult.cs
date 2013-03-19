using System;
using System.Collections.Generic;
using System.Text;

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

		internal void AddIntermediateResult(Double workDone, TimeSpan timeElapsed)
		{
			IntermediateResults.Add(new IntermediateTestResult(workDone, timeElapsed));
			TotalWorkDone = workDone; 
		}

		public override string ToString()
		{
			return ToString(false);
		}

		public string ToString(bool shortFormat)
		{
			var builder = new StringBuilder();
			builder.AppendFormat("Total: {0:0.000}{1}", TotalWorkDone, Environment.NewLine);
			if (!shortFormat)
				foreach (IntermediateTestResult result in IntermediateResults)
					builder.AppendLine(result.ToString());
			return builder.ToString();
		}

		public double TotalWorkDone { get; private set; }
		public List<IntermediateTestResult> IntermediateResults { get; private set; }
	}
}