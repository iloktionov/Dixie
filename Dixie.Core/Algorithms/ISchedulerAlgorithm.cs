using System.Collections.Generic;

namespace Dixie.Core
{
	public interface ISchedulerAlgorithm
	{
		IEnumerable<TaskAssignation> AssignNodes(List<NodeInfo> aliveNodes, List<Task> pendingTasks); 
		void Reset();
		string Name { get; set; }
	}
}
