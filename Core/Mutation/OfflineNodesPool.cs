using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dixie.Core
{
	internal class OfflineNodesPool
	{
		public OfflineNodesPool()
		{
			watch = Stopwatch.StartNew();
			offlineNodes = new List<OfflineNodeInfo>();
		}

		public void Put(Node node, INode parent, NodeFailureType failureType, TimeSpan offlineTime)
		{
			offlineNodes.Add(new OfflineNodeInfo(node, parent, watch.Elapsed + offlineTime, failureType));
		}

		public IEnumerable<OfflineNodeInfo> PopNodesReadyForReturn()
		{
			List<OfflineNodeInfo> result = null;
			foreach (OfflineNodeInfo info in offlineNodes)
				if (watch.Elapsed >= info.ReturnTimestamp)
				{
					if (result == null)
						result = new List<OfflineNodeInfo>();
					result.Add(info);
				}
			if (result == null)
				return Enumerable.Empty<OfflineNodeInfo>();
			foreach (OfflineNodeInfo info in result)
				offlineNodes.Remove(info);
			return result;
		}

		internal List<OfflineNodeInfo> OfflineNodes
		{
			get { return offlineNodes; }
		}

		private readonly Stopwatch watch;
		private readonly List<OfflineNodeInfo> offlineNodes;
	}
}