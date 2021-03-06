﻿using System;
using QuickGraph;

namespace Dixie.Core
{
	[Serializable]
	internal class NetworkLink : Edge<INode>
	{
		public NetworkLink(INode source, INode target, TimeSpan latency)
			: base(source, target)
		{
			Latency = latency;
		}

		public TimeSpan Latency { get; set; }

		public override string ToString()
		{
			return String.Format("[{0} ----({1:0.00}ms)---> {2}]", Source.Id, Latency.TotalMilliseconds, Target.Id);
		}
	}
}