using System;

namespace Dixie.Core
{
	internal class TopologyConfigurator : ITopologyConfigurator
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
			return random.NextDouble(topologySettings.MinPerformance, topologySettings.MaxPerformance);
		}

		public double GenerateFailureProbability()
		{
			return random.NextDouble(topologySettings.MinFailureProbability, topologySettings.MaxFailureProbability);
		}

		public TimeSpan GenerateLinkLatency()
		{
			return GenerateRandomTimespan(topologySettings.MinLinkLatency, topologySettings.MaxLinkLatency);
		}

		public TimeSpan GenerateOfflineTime(NodeFailureType failureType)
		{
			switch (failureType)
			{
				case NodeFailureType.ShortTerm: return GenerateRandomTimespan(topologySettings.MinShortTermOffline, topologySettings.MaxShortTermOffline);
				case NodeFailureType.LongTerm: return GenerateRandomTimespan(topologySettings.MinLongTermOffline, topologySettings.MaxLongTermOffline);
				default:
					throw new ArgumentOutOfRangeException("failureType");
			}
		}

		private TimeSpan GenerateRandomTimespan(TimeSpan from, TimeSpan to)
		{
			return TimeSpan.FromMilliseconds(random.NextDouble(from.TotalMilliseconds, to.TotalMilliseconds));
		}

		private readonly TopologySettings topologySettings;
		private readonly Random random;
	}
}