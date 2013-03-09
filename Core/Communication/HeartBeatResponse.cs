using System.Collections.Generic;

namespace Dixie.Core
{
	public class HeartBeatResponse
	{
		public HeartBeatResponse(List<ComputationalTask> tasks = null)
		{
			Tasks = tasks;
		}

		public List<ComputationalTask> Tasks { get; private set; }
	}
}