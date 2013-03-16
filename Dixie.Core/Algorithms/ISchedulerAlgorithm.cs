using System.Collections.Generic;

namespace Dixie.Core
{
	public interface ISchedulerAlgorithm
	{
		void Work(List<NodeInfo> aliveNodes, TaskManager taskManager);
		void Reset();
		string Name { get; set; }
	}
}
