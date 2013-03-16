using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dixie.Core
{
	internal partial class NodesManager
	{
		public NodesManager(TimeSpan deadabilityThreshold, ILog log)
		{
			this.deadabilityThreshold = deadabilityThreshold;
			this.log = new PrefixedILogWrapper(log, "NodesManager");
			aliveNodeInfos = new Dictionary<Guid, NodeInfo>();
			offlineNodeInfos = new Dictionary<Guid, NodeInfo>();
			watch = Stopwatch.StartNew();
		}

		public void HandleHeartBeatMessage(HeartBeatMessage message)
		{
			NodeInfo nodeInfo;
			if (aliveNodeInfos.TryGetValue(message.NodeId, out nodeInfo))
			{
				// (iloktionov): "Живая" нода посылает очередной пинг.
				nodeInfo.Performance = message.Performance;
				nodeInfo.WorkBufferSize = message.WorkBufferSize;
				nodeInfo.CommunicationLatency = message.CommunicationLatency;
				nodeInfo.LastPingTimestamp = watch.Elapsed;
			}
			else if (offlineNodeInfos.TryGetValue(message.NodeId, out nodeInfo))
			{
				// (iloktionov): Нода возвращается из оффлайна.
				nodeInfo.Performance = message.Performance;
				nodeInfo.WorkBufferSize = message.WorkBufferSize;
				nodeInfo.CommunicationLatency = message.CommunicationLatency;

				TimeSpan pingTSBeforeFailure = nodeInfo.LastPingTimestamp;
				nodeInfo.LastPingTimestamp = watch.Elapsed;
				nodeInfo.FailureHistory.AddFailure(pingTSBeforeFailure, nodeInfo.LastPingTimestamp - pingTSBeforeFailure);

				offlineNodeInfos.Remove(message.NodeId);
				aliveNodeInfos.Add(message.NodeId, nodeInfo);
			}
			else
			{
				// (iloktionov): Пришёл первый пинг от абсолютно новой ноды.
				nodeInfo = new NodeInfo(message.NodeId, message.Performance, message.CommunicationLatency, message.WorkBufferSize, watch.Elapsed);
				aliveNodeInfos.Add(message.NodeId, nodeInfo);
			}
		}

		public List<Guid> RemoveDeadsOrNull()
		{
			TimeSpan timeElapsed = watch.Elapsed;
			List<Guid> result = null;
			foreach (KeyValuePair<Guid, NodeInfo> pair in aliveNodeInfos)
				if (timeElapsed - pair.Value.LastPingTimestamp > deadabilityThreshold)
				{
					if (result == null)
						result = new List<Guid>();
					result.Add(pair.Key);
					FailuresCount++;
				}
			if (result != null)
			{
				foreach (Guid nodeId in result)
				{
					NodeInfo info = aliveNodeInfos[nodeId];
					aliveNodeInfos.Remove(nodeId);
					offlineNodeInfos.Add(nodeId, info);
				}
				LogDeadsCount(result.Count);
			}
			return result;
		}

		// (iloktionov): GarbageCollector гарантирует, что со времени пропажи ноды прошло гораздо больше, чем deadabilityThreshold.
		// Поэтому удалять из alives не требуется.
		public void CollectGarbage(IEnumerable<Guid> permanentlyDeletedNodes)
		{
			foreach (Guid nodeId in permanentlyDeletedNodes)
				offlineNodeInfos.Remove(nodeId);
		}

		public List<NodeInfo> GetAliveNodeInfos()
		{
			return new List<NodeInfo>(aliveNodeInfos.Values);
		}

		internal int AliveNodesCount
		{
			get { return aliveNodeInfos.Count; }
		}

		internal int OfflineNodesCount
		{
			get { return offlineNodeInfos.Count; }
		}

		internal int FailuresCount { get; private set; }

		#region Logging
		private void LogDeadsCount(int count)
		{
			log.Info("Removed {0} dead node(s).", count);
		}
		#endregion

		private readonly Dictionary<Guid, NodeInfo> aliveNodeInfos;
		private readonly Dictionary<Guid, NodeInfo> offlineNodeInfos;
		private readonly Stopwatch watch;
		private readonly TimeSpan deadabilityThreshold;
		private readonly ILog log;
	}
}
