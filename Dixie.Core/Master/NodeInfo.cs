using System;

namespace Dixie.Core
{
	public class NodeInfo
	{
		public NodeInfo(Guid id, double performance, TimeSpan communicationLatency, int workBufferSize, TimeSpan lastPingTimestamp, TimeSpan availabilityTime)
		{
			Id = id;
			Performance = performance;
			CommunicationLatency = communicationLatency;
			WorkBufferSize = workBufferSize;
			LastPingTimestamp = lastPingTimestamp;
			AvailabilityTime = availabilityTime;
		}

		public Guid Id { get; private set; }
		public Double Performance { get; set; }
		public TimeSpan CommunicationLatency { get; set; }
		public int WorkBufferSize { get; set; }

		/// <summary>
		/// Время оставшееся до полного освобождения узла.
		/// </summary>
		public TimeSpan AvailabilityTime { get; set; }

		internal TimeSpan LastPingTimestamp { get; set; }

		public NodeFailureHistory FailureHistory
		{
			get { return failureHistory ?? (failureHistory = new NodeFailureHistory()); }
		}

		private NodeFailureHistory failureHistory;
	}
}