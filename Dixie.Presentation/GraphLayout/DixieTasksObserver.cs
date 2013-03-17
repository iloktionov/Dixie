using System.Linq;
using Dixie.Core;

namespace Dixie.Presentation
{
	internal class DixieTasksObserver
	{
		public DixieTasksObserver(DixieModel model)
		{
			this.model = model;
		}

		public void TryUpdateTaskStates(Master master)
		{
			if (master == null)
				Reset();
			else model.TaskStates = master
				.GetTaskStates()
				.OrderByDescending(state => state.Task.Volume)
				.ToList();
		}

		public void Reset()
		{
			model.TaskStates = new TaskState[]{};
		}

		private readonly DixieModel model;
	}
}