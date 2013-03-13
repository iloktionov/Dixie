using System.Collections.Generic;

namespace Dixie.Core
{
	internal class GridInfo
	{
		public GridInfo(List<NodeInfo> aliveNodes)
		{
			AliveNodes = aliveNodes;
		}

		public List<NodeInfo> AliveNodes { get; private set; }
	}
}