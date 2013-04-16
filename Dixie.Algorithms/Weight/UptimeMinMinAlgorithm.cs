using System;
using System.ComponentModel.Composition;

namespace Dixie.Core
{
	[Export(typeof(ISchedulerAlgorithm))]
	internal class UptimeMinMinAlgorithm : WeightedMinAlgorithmBase
	{
		public UptimeMinMinAlgorithm()
			: base("UptimeMinMinAlgorithm", new UptimeWeightSelector()) { }

		protected override double GetInitialBestCompletionTime()
		{
			return Double.MaxValue;
		}

		protected override bool IsBetterTime(double completionTime, double currentBestTime)
		{
			return completionTime < currentBestTime;
		}
	}
}