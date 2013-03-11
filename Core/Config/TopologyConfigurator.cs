using System;

namespace Dixie.Core
{
	public class TopologyConfigurator : ITopologyConfigurator
	{
		public TopologyConfigurator(TopologySettings topologySettings, Random random)
		{
			this.topologySettings = topologySettings;
			this.random = random;
		}

		public TopologyConfigurator(Random random)
			: this (TopologySettings.GetInstance(), random) { }

		public TopologyConfigurator()
			: this (new Random()) { }

		public int GenerateChildrenCount()
		{
			return random.Next(topologySettings.MinChildrenCount, topologySettings.MaxChildrenCount + 1);
		}

		public double GeneratePerformance()
		{
			return random.NextDouble() * (topologySettings.MaxPerformance - topologySettings.MinPerformance) + topologySettings.MinPerformance;
		}

		public double GenerateFailureProbability()
		{
			return random.NextDouble() * (topologySettings.MaxFailureProbability - topologySettings.MinFailureProbability) + topologySettings.MinFailureProbability;
		}

		public TimeSpan GenerateLinkLatency()
		{
			return TimeSpan.FromMilliseconds(random.NextDouble() * (topologySettings.MaxLinkLatency.TotalMilliseconds - topologySettings.MinLinkLatency.TotalMilliseconds) + topologySettings.MinLinkLatency.TotalMilliseconds);
		}

		private readonly TopologySettings topologySettings;
		private readonly Random random;
	}
}