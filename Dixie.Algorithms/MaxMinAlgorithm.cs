using System;
using System.ComponentModel.Composition;

namespace Dixie.Core
{
	[Export(typeof(ISchedulerAlgorithm))]
	internal class MaxMinAlgorithm : MinAlgorithmBase
	{
		public MaxMinAlgorithm()
			: base("MaxMinAlgorithm") { }

		protected override double GetInitialBestCompletionTime()
		{
			return Double.MinValue;
		}

		protected override bool IsBetterTime(double completionTime, double currentBestTime)
		{
			return completionTime > currentBestTime;
		}
	}
}