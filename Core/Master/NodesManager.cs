using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dixie.Core
{
	internal class NodesManager
	{
		public NodesManager()
		{
			watch = Stopwatch.StartNew();
		}

		public void HandleHeartBeatMessage(HeartBeatMessage message)
		{
		}

		public void UpdateState()
		{
			
		}

		private readonly TimeSpan deadabilityThreshold;
		private readonly Stopwatch watch;
	}
}
