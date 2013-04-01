using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Dixie.Core
{
	internal partial class TaskManager
	{
		[TestFixture]
		internal class TaskManager_Tests
		{
			[Test]
			public void Test_PutTasks_1()
			{
				var manager = new TaskManager(log);
				manager.PutTasks(GenerateTasks(10));
				Assert.AreEqual(10, manager.taskStates.Count);
				Assert.AreEqual(10, manager.GetPendingTasks().Count);
			}

			[Test]
			public void Test_PutTasks_2()
			{
				var manager = new TaskManager(log);
				List<Task> tasks = GenerateTasks(100);
				manager.PutTasks(tasks);
				Assert.AreEqual(100, manager.taskStates.Count);

				manager.ReportTasksCompletion(Guid.Empty, new List<Guid>{tasks[0].Id});
				Assert.AreEqual(1, manager.completedTasksCount);
				
				manager.PutTasks(GenerateTasks(10));
				Assert.AreEqual(10, manager.taskStates.Count);
				Assert.AreEqual(0, manager.completedTasksCount);
			}

			[Test]
			public void Test_GetPendingTasks()
			{
				var manager = new TaskManager(log);
				manager.PutTasks(GenerateTasks(10));
				for (int i = 0; i < 10; i++)
				{
					manager.taskStates.First(pair => pair.Value.Status == TaskStatus.Pending).Value.ReportCompletion(Guid.Empty);
					Assert.AreEqual(10 - i - 1, manager.GetPendingTasks().Count);
				}
			}

			[Test]
			public void Test_MultipleNodesCompleteEachTask()
			{
				var manager = new TaskManager(log);
				Assert.True(manager.NeedsRefill());
				List<Task> tasks = GenerateTasks(10, 10);
				manager.PutTasks(tasks);
				Assert.False(manager.NeedsRefill());

				for (int i = 0; i < tasks.Count; i++)
				{
					Task task = tasks[i];
					manager.ReportTasksCompletion(Guid.NewGuid(), new List<Guid>{ task.Id });
					manager.ReportTasksCompletion(Guid.NewGuid(), new List<Guid>{ task.Id });
					manager.ReportTasksCompletion(Guid.NewGuid(), new List<Guid>{ task.Id });
					manager.ReportTasksCompletion(Guid.NewGuid(), new List<Guid>{ task.Id });
					manager.ReportTasksCompletion(Guid.NewGuid(), new List<Guid>{ task.Id });
					Assert.AreEqual(i + 1, manager.completedTasksCount);
					Assert.AreEqual((i + 1) * 10, manager.TotalWorkDone);
					if (i < tasks.Count - 1)
						Assert.False(manager.NeedsRefill());
				}
				Assert.True(manager.NeedsRefill());
			}

			[Test]
			public void Test_AssignationProcess()
			{
				var manager = new TaskManager(log);
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
				var manager = new TaskManager(log);
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
				var manager = new TaskManager(log);
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

			private List<Task> GenerateTasks(int count, int volume = 100)
			{
				return Enumerable.Range(1, count).Select(i => new Task(volume)).ToList();
			}

			private readonly ILog log = new ColorConsoleLog(true);
		}
	}
}