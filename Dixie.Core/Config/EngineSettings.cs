﻿using System;
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

		public Double MinTaskVolume = 5 * 1000d;
		public Double MaxTaskVolume = 120 * 1000d;

		public TimeSpan TopologyMutatorRunPeriod = TimeSpan.FromMilliseconds(50);
		public TimeSpan SchedulingAlgorithmRunPeriod = TimeSpan.FromMilliseconds(150);
		public TimeSpan TasksGeneratorRunPeriod = TimeSpan.FromMilliseconds(1);
		public TimeSpan GarbageCollectorRunPeriod = TimeSpan.FromSeconds(30);
	}
}