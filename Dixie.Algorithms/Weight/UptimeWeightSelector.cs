using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dixie.Core
{
	internal class UptimeWeightSelector : IWeightSelector
	{
		public UptimeWeightSelector()
		{
			watch = Stopwatch.StartNew();
			weights = new Dictionary<Guid, double>();
		}

		public void Reset()
		{
			watch.Restart();
		}

		public double GetWeight(Guid nodeId)
		{
			return weights[nodeId];
		}

		public void Update(List<NodeInfo> aliveNodes)
		{
			weights.Clear();
			TimeSpan totalTimeElapsed = watch.Elapsed;
			foreach (NodeInfo aliveNode in aliveNodes)
				weights.Add(aliveNode.Id, GetWeight(aliveNode, totalTimeElapsed));
		}

		private static Double GetWeight(NodeInfo aliveNode, TimeSpan totalTimeElapsed)
		{
			if (aliveNode.FailureHistory.Failures.Count <= 0)
				return 1;
			TimeSpan downtime = aliveNode.FailureHistory.Downtime();
			TimeSpan uptime = totalTimeElapsed - downtime;
			return uptime > TimeSpan.Zero ? totalTimeElapsed.TotalMilliseconds / uptime.TotalMilliseconds : 1;
		}

		private readonly Stopwatch watch;
		private readonly Dictionary<Guid, Double> weights;
	}
}