using System.Collections.Generic;

namespace Dixie.Core
{
	public interface ISchedulerAlgorithm
	{
		void Work(List<NodeInfo> aliveNodes, TaskManager taskManager);
		string Name { get; }
	}
}
