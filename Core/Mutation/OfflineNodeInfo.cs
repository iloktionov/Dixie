using System;

namespace Dixie.Core
{
	internal class OfflineNodeInfo
	{
		public OfflineNodeInfo(Node offlineNode, INode parentNode, TimeSpan returnTimestamp)
		{
			OfflineNode = offlineNode;
			ParentNode = parentNode;
			ReturnTimestamp = returnTimestamp;
		}

		public Node OfflineNode { get; private set; }
		public INode ParentNode { get; private set; }
		public TimeSpan ReturnTimestamp { get; private set; }
	}
}