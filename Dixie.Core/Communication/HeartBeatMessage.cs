﻿using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	internal class HeartBeatMessage
	{
		public HeartBeatMessage(Guid nodeId, double performance, int workBufferSize = 0, List<Guid> completedTasks = null)
		{
			NodeId = nodeId;
			Performance = performance;
			WorkBufferSize = workBufferSize;
			CompletedTasks = completedTasks;
			CommunicationLatency = TimeSpan.Zero;
		}

		public Guid NodeId { get; private set; }
		public Double Performance { get; private set; }
		public List<Guid> CompletedTasks { get; private set; }
		public int WorkBufferSize { get; private set; }
		public TimeSpan CommunicationLatency { get; set; }
	}
}