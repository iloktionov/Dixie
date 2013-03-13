using System;
using Configuration;

namespace Dixie.Core
{
	[Serializable]
	[Configuration("dixie.Engine", false)]
	internal class EngineSettings
	{
		public static EngineSettings GetInstance()
		{
			return Configuration<EngineSettings>.Get();
		}

		public Double RemoveNodesProbability = 0.05d;
		public Double AddNodesProbability = 0.05d;
		public TimeSpan HeartBeatPeriod = TimeSpan.FromMilliseconds(50);
		public TimeSpan DeadabilityThreshold = TimeSpan.FromMilliseconds(250);
		public Double MinTaskVolume = 500 * 1000d;
		public Double MaxTaskVolume = 4000 * 1000d;
	}
}