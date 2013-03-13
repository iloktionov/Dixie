using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	public class HeartBeatResponse
	{
		public HeartBeatResponse(Guid nodeId, List<ComputationalTask> tasks = null)
		{
			NodeId = nodeId;
			Tasks = tasks;
		}

		public Guid NodeId { get; private set; }
		public List<ComputationalTask> Tasks { get; private set; }
	}
}