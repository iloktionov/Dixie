using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	internal class HeartBeatMessage
	{
		public HeartBeatMessage(Guid nodeId, double performance, int workBufferSize, List<Guid> completedTasks, TimeSpan availabilityTime)
		{
			AvailabilityTime = availabilityTime;
			NodeId = nodeId;
			Performance = performance;
			WorkBufferSize = workBufferSize;
			CompletedTasks = completedTasks;
			CommunicationLatency = TimeSpan.Zero;
		}

		public HeartBeatMessage(Guid nodeId, double performance)
			: this (nodeId, performance, 0, null, TimeSpan.Zero) { }

		public Guid NodeId { get; private set; }
		public Double Performance { get; private set; }
		public List<Guid> CompletedTasks { get; private set; }
		public int WorkBufferSize { get; private set; }
		public TimeSpan CommunicationLatency { get; set; }
		public TimeSpan AvailabilityTime { get; private set; }
	}
}