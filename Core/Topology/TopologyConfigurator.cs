using System;

namespace Dixie.Core
{
	public class TopologyConfigurator : ITopologyConfigurator
	{
		public TopologyConfigurator(Range<int> chilrenCountRange, Range<double> performanceRange, Range<double> failureProbabilityRange, Range<TimeSpan> latencyRange)
		{
			this.chilrenCountRange = chilrenCountRange;
			this.performanceRange = performanceRange;
			this.failureProbabilityRange = failureProbabilityRange;
			this.latencyRange = latencyRange;
			random = new Random();
		}

		public int GenerateChildrenCount()
		{
			return random.Next(chilrenCountRange.From, chilrenCountRange.To + 1);
		}

		public double GeneratePerformance()
		{
			return random.NextDouble() * (performanceRange.To - performanceRange.From) + performanceRange.From;
		}

		public double GenerateFailureProbability()
		{
			return random.NextDouble() * (failureProbabilityRange.To - failureProbabilityRange.From) + failureProbabilityRange.From;
		}

		public TimeSpan GenerateLinkLatency()
		{
			return TimeSpan.FromMilliseconds(random.NextDouble() * (latencyRange.To.TotalMilliseconds - latencyRange.From.TotalMilliseconds) + latencyRange.From.TotalMilliseconds);
		}

		public static TopologyConfigurator CreateDefault()
		{
			return new TopologyConfigurator(new Range<int>(3, 5), new Range<double>(500, 10 * 1000), new Range<double>(0, 0.05), new Range<TimeSpan>(TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(5)));
		}

		private readonly Range<int> chilrenCountRange;
		private readonly Range<double> performanceRange;
		private readonly Range<double> failureProbabilityRange;
		private readonly Range<TimeSpan> latencyRange;
		private readonly Random random;
	}
}