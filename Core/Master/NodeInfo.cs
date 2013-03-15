using System;

namespace Dixie.Core
{
	public class NodeInfo
	{
		internal NodeInfo(Guid id, double performance, TimeSpan communicationLatency, int workBufferSize, TimeSpan lastPingTimestamp)
		{
			Id = id;
			Performance = performance;
			CommunicationLatency = communicationLatency;
			WorkBufferSize = workBufferSize;
			LastPingTimestamp = lastPingTimestamp;
		}

		public Guid Id { get; private set; }
		public Double Performance { get; set; }
		public TimeSpan CommunicationLatency { get; set; }
		public int WorkBufferSize { get; set; }
		public TimeSpan LastPingTimestamp { get; set; }

		public NodeFailureHistory FailureHistory
		{
			get { return failureHistory ?? (failureHistory = new NodeFailureHistory()); }
		}

		private NodeFailureHistory failureHistory;
	}
}