using System;
using Dixie.Core;
using QuickGraph;

namespace Dixie.Presentation
{
	internal class DixieGraph : BidirectionalGraph<NodeState, DixieEdge> { }

	internal class DixieEdge : Edge<NodeState>
	{
		public DixieEdge(NodeState source, NodeState target, TimeSpan latency)
			: base(source, target)
		{
			Latency = latency;
		}

		public TimeSpan Latency { get; set; }
	}
}