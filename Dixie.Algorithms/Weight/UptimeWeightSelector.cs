using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dixie.Core
{
	internal class UptimeWeightSelector : IWeightSelector
	{
		public UptimeWeightSelector()
		{
			weights = new Dictionary<Guid, double>();
		}

		public void Reset()
		{
		}

		public double GetWeight(Guid nodeId)
		{
			return weights[nodeId];
		}

		public void Update(List<NodeInfo> aliveNodes)
		{
			weights.Clear();
			foreach (NodeInfo aliveNode in aliveNodes)
				weights.Add(aliveNode.Id, GetWeight(aliveNode));
		}

		private static Double GetWeight(NodeInfo aliveNode)
		{
			if (aliveNode.FailureHistory.Failures.Count <= 0)
				return 1;
			TimeSpan totalTime = aliveNode.LastPingTimestamp - aliveNode.FirstPingTimestamp;
			TimeSpan downtime = aliveNode.FailureHistory.Downtime();
			TimeSpan uptime = totalTime - downtime;
			return uptime > TimeSpan.Zero ? totalTime.TotalMilliseconds / uptime.TotalMilliseconds : 1;
		}

		private readonly Dictionary<Guid, Double> weights;
	}
}