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

		public IEnumerable<OfflineNodeInfo> GetNodesReadyForReturn()
		{
			return offlineNodes.Where(info => info.ReturnTimestamp <= watch.Elapsed);
		}

		internal List<OfflineNodeInfo> OfflineNodes
		{
			get { return offlineNodes; }
		}

		private readonly Stopwatch watch;
		private readonly List<OfflineNodeInfo> offlineNodes;
	}
}