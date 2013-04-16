using System;
using System.ComponentModel.Composition;

namespace Dixie.Core
{
	[Export(typeof(ISchedulerAlgorithm))]
	internal class UptimeMaxMinAlgorithm : WeightedMinAlgorithmBase
	{
		public UptimeMaxMinAlgorithm()
			: base("UptimeMaxMinAlgorithm", new UptimeWeightSelector()) { }

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