using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Dixie.Core
{
	public partial class TaskManager
	{
		[TestFixture]
		internal class TaskManager_Tests
		{
			[Test]
			public void Test_PutTasks()
			{
				var manager = new TaskManager();
				manager.PutTasks(GenerateTasks(10));
				Assert.AreEqual(10, manager.taskStates.Count);
				Assert.AreEqual(10, manager.GetPendingTasks().Count);
			}

			[Test]
			public void Test_NeedsRefill()
			{
				var manager = new TaskManager();
				Assert.True(manager.NeedsRefill());
				List<Task> tasks = GenerateTasks(10);
				manager.PutTasks(tasks);
				Assert.False(manager.NeedsRefill());
				manager.ReportTasksCompletion(Guid.NewGuid(), tasks.GetRange(0, 9).Select(task => task.Id).ToList());
				Assert.False(manager.NeedsRefill());
				manager.ReportTasksCompletion(Guid.NewGuid(), tasks.GetRange(9, 1).Select(task => task.Id).ToList());
				Assert.True(manager.NeedsRefill());
			}

			[Test]
			public void Test_AssignationProcess()
			{
				var manager = new TaskManager();
				List<Task> tasks = GenerateTasks(10);
				Guid node1 = Guid.NewGuid();
				Guid node2 = Guid.NewGuid();
				manager.PutTasks(tasks);

				manager.AssignNodeToTask(tasks[0], node1);
				manager.AssignNodeToTask(tasks[0], node2);
				manager.AssignNodeToTask(tasks[1], node1);
				manager.AssignNodeToTask(tasks[1], node2);
				Assert.AreEqual(2, manager.assignationsMap.Count);

				List<Task> tasksForNode1 = manager.GetTasksForNodeOrNull(node1);
				List<Task> tasksForNode2 = manager.GetTasksForNodeOrNull(node2);
				Assert.AreEqual(2, tasksForNode1.Count);
				Assert.AreEqual(2, tasksForNode2.Count);
				Assert.True(ReferenceEquals(tasks[0], tasksForNode1[0]));
				Assert.True(ReferenceEquals(tasks[0], tasksForNode2[0]));
				Assert.True(ReferenceEquals(tasks[1], tasksForNode1[1]));
				Assert.True(ReferenceEquals(tasks[1], tasksForNode2[1]));
				Assert.AreEqual(0, manager.assignationsMap.Count);

				Assert.Null(manager.GetTasksForNodeOrNull(node1));
				Assert.Null(manager.GetTasksForNodeOrNull(node2));
			}

			[Test]
			public void Test_ReportTaskCompletion()
			{
				var manager = new TaskManager();
				List<Task> tasks = GenerateTasks(10);
				manager.PutTasks(tasks);
				manager.ReportTasksCompletion(Guid.NewGuid(), tasks.GetRange(0, 5).Select(task => task.Id).ToList());
				manager.ReportTasksCompletion(Guid.NewGuid(), null);
				manager.ReportTasksCompletion(Guid.NewGuid(), GenerateTasks(10).Select(task => task.Id).ToList());
				Assert.AreEqual(5, manager.GetPendingTasks().Count);
			}

			[Test]
			public void Test_ReportDeadNodes()
			{
				var manager = new TaskManager();
				List<Task> tasks = GenerateTasks(10);
				Guid node = Guid.NewGuid();
				manager.PutTasks(tasks);

				manager.AssignNodeToTask(tasks[0], node);
				Assert.AreEqual(9, manager.GetPendingTasks().Count);
				manager.ReportDeadNodes(null);
				Assert.AreEqual(9, manager.GetPendingTasks().Count);
				manager.ReportDeadNodes(new List<Guid>{node});
				manager.ReportDeadNodes(new List<Guid>{node});
				manager.ReportDeadNodes(new List<Guid>{node});
				Assert.AreEqual(10, manager.GetPendingTasks().Count);
			}

			private List<Task> GenerateTasks(int count)
			{
				return Enumerable.Range(1, count).Select(i => new Task(i)).ToList();
			}
		}
	}
}