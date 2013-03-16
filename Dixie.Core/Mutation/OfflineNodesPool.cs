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
			HashSet<OfflineNodeInfo> result = null;
			TimeSpan timeElapsed = watch.Elapsed;
			for (int index = 0; index < offlineNodes.Count; index++)
			{
				OfflineNodeInfo info = offlineNodes[index];
				if (timeElapsed >= info.ReturnTimestamp)
				{
					if (result == null)
						result = new HashSet<OfflineNodeInfo>(equalityComparer);
					result.Add(info);
				}
			}
			if (result == null)
				return Enumerable.Empty<OfflineNodeInfo>();
			offlineNodes.RemoveAll(result.Contains);
			return result;
		}

		internal List<OfflineNodeInfo> OfflineNodes
		{
			get { return offlineNodes; }
		}

		private readonly Stopwatch watch;
		private readonly List<OfflineNodeInfo> offlineNodes;
		private static readonly IEqualityComparer<OfflineNodeInfo> equalityComparer = new OfflineInfoEqualityComparer();

		#region Equality comparer
		private class OfflineInfoEqualityComparer : IEqualityComparer<OfflineNodeInfo>
		{
			public bool Equals(OfflineNodeInfo x, OfflineNodeInfo y)
			{
				return ReferenceEquals(x, y);
			}

			public int GetHashCode(OfflineNodeInfo obj)
			{
				return obj.GetHashCode();
			}
		} 
		#endregion
	}
}