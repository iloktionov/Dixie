using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dixie.Core
{
	internal partial class NodesManager
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

		public List<Guid> RemoveDeadsOrNull()
		{
			TimeSpan timeElapsed = watch.Elapsed;
			List<Guid> result = null;
			foreach (KeyValuePair<Guid, TimeSpan> pair in hbmTimestamps)
				if (timeElapsed - pair.Value > deadabilityThreshold)
				{
					aliveNodeInfos.Remove(pair.Key);
					if (result == null)
						result = new List<Guid>();
					result.Add(pair.Key);
				}
			return result;
		}

		public IEnumerable<NodeInfo> GetAliveNodeInfos()
		{
			return aliveNodeInfos.Values;
		}

		private readonly Dictionary<Guid, TimeSpan> hbmTimestamps;
		private readonly Dictionary<Guid, NodeInfo> aliveNodeInfos;
		private readonly Stopwatch watch;
		private readonly TimeSpan deadabilityThreshold;
	}
}
