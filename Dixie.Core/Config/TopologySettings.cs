using System;
using Configuration;

namespace Dixie.Core
{
	[Serializable]
	[Configuration("dixie.Topology", false)]
	public class TopologySettings
	{
		public static TopologySettings GetInstance()
		{
			return Configuration<TopologySettings>.Get();
		}

		public double MinPerformance = 5;
		public double MaxPerformance = 40;

		public double MinFailureProbability = 0;
		public double MaxFailureProbability = 0.075;

		public TimeSpan MinLinkLatency = TimeSpan.FromMilliseconds(0.1);
		public TimeSpan MaxLinkLatency = TimeSpan.FromMilliseconds(5);

		public TimeSpan MinShortTermOffline = TimeSpan.FromSeconds(2);
		public TimeSpan MaxShortTermOffline = TimeSpan.FromSeconds(5);
		public TimeSpan MinLongTermOffline = TimeSpan.FromMinutes(1);
		public TimeSpan MaxLongTermOffline = TimeSpan.FromMinutes(3);
	}
}