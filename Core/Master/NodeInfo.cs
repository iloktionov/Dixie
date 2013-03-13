using System;

namespace Dixie.Core
{
	public class NodeInfo
	{
		public NodeInfo(Guid id, double performance, TimeSpan communicationLatency, int workBufferSize)
		{
			Id = id;
			Performance = performance;
			CommunicationLatency = communicationLatency;
			WorkBufferSize = workBufferSize;
		}

		public Guid Id { get; set; }
		public Double Performance { get; set; }
		public TimeSpan CommunicationLatency { get; set; }
		public int WorkBufferSize { get; set; }
	}
}