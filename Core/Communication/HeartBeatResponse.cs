using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	internal class HeartBeatResponse
	{
		public HeartBeatResponse(Guid nodeId, List<Task> tasks = null)
		{
			NodeId = nodeId;
			Tasks = tasks;
		}

		public Guid NodeId { get; private set; }
		public List<Task> Tasks { get; private set; }
	}
}