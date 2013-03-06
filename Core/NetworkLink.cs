using System;
using QuickGraph;

namespace Dixie.Core
{
	[Serializable]
	public class NetworkLink : Edge<INode>
	{
		public NetworkLink(INode source, INode target, TimeSpan latency)
			: base(source, target)
		{
			Latency = latency;
		}

		public TimeSpan Latency { get; private set; }

		public override string ToString()
		{
			return String.Format("[{0} ----({1:0.00}ms)---> {2}]", Source.Id, Latency.TotalMilliseconds, Target.Id);
		}
	}
}