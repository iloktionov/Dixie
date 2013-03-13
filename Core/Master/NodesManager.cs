﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dixie.Core
{
	internal class NodesManager
	{
		public NodesManager(TimeSpan deadabilityThreshold)
		{
			this.deadabilityThreshold = deadabilityThreshold;
			hbmTimestamps = new Dictionary<Guid, TimeSpan>();
			aliveNodeInfos = new Dictionary<Guid, NodeInfo>();
			watch = Stopwatch.StartNew();
		}

		public void HandleHeartBeatMessage(HeartBeatMessage message)
		{
			hbmTimestamps[message.NodeId] = watch.Elapsed;
			NodeInfo info;
			if (aliveNodeInfos.TryGetValue(message.NodeId, out info))
			{
				// Нода живая, это обычный пинг.
				info.Performance = message.Performance;
				info.WorkBufferSize = message.WorkBufferSize;
				info.CommunicationLatency = message.CommunicationLatency;
			}
			else
			{
				// Появилась новая нода, или вернулась из оффлайна старая.
				info = new NodeInfo(message.NodeId, message.Performance, message.CommunicationLatency, message.WorkBufferSize);
				aliveNodeInfos.Add(message.NodeId, info);
			}
		}

		public void UpdateState()
		{
			TimeSpan timeElapsed = watch.Elapsed;
			foreach (KeyValuePair<Guid, TimeSpan> pair in hbmTimestamps)
				if (timeElapsed - pair.Value > deadabilityThreshold)
					aliveNodeInfos.Remove(pair.Key);
		}

		public GridInfo GetGridInfo()
		{
			return new GridInfo(new List<NodeInfo>(aliveNodeInfos.Values));
		}

		private readonly Dictionary<Guid, TimeSpan> hbmTimestamps;
		private readonly Dictionary<Guid, NodeInfo> aliveNodeInfos;
		private readonly Stopwatch watch;
		private readonly TimeSpan deadabilityThreshold;
	}
}
