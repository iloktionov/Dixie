using System;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class TaskState_Tests
	{
		[Test]
		public void Test_AssignNode()
		{
			var state = new TaskState(new Task(1.0d));
			Assert.AreEqual(TaskStatus.Pending, state.Status);
			state.AssignNode(Guid.NewGuid());
			Assert.AreEqual(TaskStatus.Assigned, state.Status);
			state.AssignNode(Guid.NewGuid());
			state.AssignNode(Guid.NewGuid());
			Guid nodeId = Guid.NewGuid();
			state.AssignNode(nodeId);
			Assert.AreEqual(4, state.AssignedNodes.Count);
			Assert.Throws<InvalidOperationException>(() => state.AssignNode(nodeId));
		}

		[Test]
		public void Test_ReportNodeFailure()
		{
			var state = new TaskState(new Task(1.0d));
			Guid node1 = Guid.NewGuid();
			Guid node2 = Guid.NewGuid();
			Guid node3 = Guid.NewGuid();
			state.AssignNode(node1);
			state.AssignNode(node2);
			state.AssignNode(node3);
			Assert.AreEqual(TaskStatus.Assigned, state.Status);
			state.ReportNodeFailure(node1);
			Assert.AreEqual(TaskStatus.Assigned, state.Status);
			state.ReportNodeFailure(node2);
			Assert.AreEqual(TaskStatus.Assigned, state.Status);
			state.ReportNodeFailure(node3);
			Assert.AreEqual(TaskStatus.Pending, state.Status);
			Assert.AreEqual(0, state.AssignedNodes.Count);
		}

		[Test]
		public void Test_ReportCompletion()
		{
			var state = new TaskState(new Task(1.0d));
			Guid node1 = Guid.NewGuid();
			state.AssignNode(node1);
			Assert.AreEqual(TaskStatus.Assigned, state.Status);
			Assert.True(state.ReportCompletion(Guid.Empty));
			Assert.AreEqual(TaskStatus.Completed, state.Status);
			Assert.False(state.ReportCompletion(Guid.Empty));
			Assert.False(state.ReportCompletion(Guid.Empty));
			Assert.AreEqual(TaskStatus.Completed, state.Status);
			Assert.Throws<InvalidOperationException>(() => state.AssignNode(Guid.NewGuid()));
		}
	}
}