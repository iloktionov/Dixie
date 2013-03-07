using System;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class WorkBuffer_Tests
	{
		[Test]
		public void Test_CorrectWork()
		{
			var buffer = new WorkBuffer();
			Guid task1 = Guid.NewGuid();
			Guid task2 = Guid.NewGuid();
			Guid task3 = Guid.NewGuid();
			Guid task4 = Guid.NewGuid();
			buffer.PutTask(task1, TimeSpan.Zero);
			buffer.PutTask(task2, TimeSpan.FromMilliseconds(50));
			buffer.PutTask(task3, TimeSpan.FromMilliseconds(150));
			buffer.PutTask(task4, TimeSpan.FromMilliseconds(250));
			Assert.AreEqual(4, buffer.Size);
			

			Assert.AreEqual(task1, buffer.RemoveCompletedTasks().Single());
			Assert.AreEqual(3, buffer.Size);

			Thread.Sleep(TimeSpan.FromMilliseconds(50));
			Assert.AreEqual(task2, buffer.RemoveCompletedTasks().Single());
			Assert.AreEqual(2, buffer.Size);

			Thread.Sleep(TimeSpan.FromMilliseconds(100));
			Assert.AreEqual(task3, buffer.RemoveCompletedTasks().Single());
			Assert.AreEqual(1, buffer.Size);

			Thread.Sleep(TimeSpan.FromMilliseconds(100));
			Assert.AreEqual(task4, buffer.RemoveCompletedTasks().Single());
			Assert.AreEqual(0, buffer.Size);

			Thread.Sleep(TimeSpan.FromMilliseconds(50));
			Assert.AreEqual(0, buffer.RemoveCompletedTasks().Count);
			Assert.AreEqual(0, buffer.Size);
		}
	}
}