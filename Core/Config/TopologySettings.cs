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

		public int MinChildrenCount = 3;
		public int MaxChildrenCount = 10;

		public double MinPerformance = 500;
		public double MaxPerformance = 4000;

		public double MinFailureProbability = 0;
		public double MaxFailureProbability = 0.25;

		public TimeSpan MinLinkLatency = TimeSpan.FromMilliseconds(0.1);
		public TimeSpan MaxLinkLatency = TimeSpan.FromMilliseconds(10);

		public TimeSpan MinShortTermOffline = TimeSpan.FromSeconds(2);
		public TimeSpan MaxShortTermOffline = TimeSpan.FromSeconds(5);
		public TimeSpan MinLongTermOffline = TimeSpan.FromMinutes(1);
		public TimeSpan MaxLongTermOffline = TimeSpan.FromMinutes(3);
	}
}