using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;

namespace Dixie.Core
{
	internal partial class WorkBuffer
	{
		[TestFixture]
		internal class WorkBuffer_Tests
		{
			[Test]
			public void Test_PutTask()
			{
				var buffer = new WorkBuffer();
				for (int i = 0; i < 5; i++)
					buffer.PutTask(Guid.NewGuid(), TimeSpan.FromSeconds(1));
				Assert.AreEqual(5, buffer.Size);
				Assert.AreEqual(TimeSpan.FromSeconds(4), buffer.records.Last().Value - buffer.records.Peek().Value);
			}

			[Test]
			public void Test_CorrectWork()
			{
				var buffer = new WorkBuffer();
				Guid task1 = Guid.NewGuid();
				Guid task2 = Guid.NewGuid();
				Guid task3 = Guid.NewGuid();
				Guid task4 = Guid.NewGuid();
				Guid task5 = Guid.NewGuid();
				Guid task6 = Guid.NewGuid();
				Guid task7 = Guid.NewGuid();
				buffer.PutTask(task1, TimeSpan.Zero);
				buffer.PutTask(task2, TimeSpan.FromMilliseconds(50));
				buffer.PutTask(task3, TimeSpan.FromMilliseconds(150));
				buffer.PutTask(task4, TimeSpan.FromMilliseconds(5));
				buffer.PutTask(task5, TimeSpan.FromMilliseconds(250));
				buffer.PutTask(task6, TimeSpan.FromMilliseconds(1));
				buffer.PutTask(task7, TimeSpan.FromMilliseconds(1));
				Assert.AreEqual(7, buffer.Size);

				Assert.AreEqual(task1, buffer.PopCompletedOrNull().Single());
				Assert.AreEqual(6, buffer.Size);

				Thread.Sleep(55);
				Assert.AreEqual(task2, buffer.PopCompletedOrNull().Single());
				Assert.AreEqual(5, buffer.Size);

				Thread.Sleep(100);
				Assert.Null(buffer.PopCompletedOrNull());
				Assert.AreEqual(5, buffer.Size);

				Thread.Sleep(100);
				List<Guid> tasks = buffer.PopCompletedOrNull();
				Assert.AreEqual(2, tasks.Count);
				Assert.AreEqual(task3, tasks[0]);
				Assert.AreEqual(task4, tasks[1]);
				Assert.AreEqual(3, buffer.Size);

				Thread.Sleep(100);
				Assert.Null(buffer.PopCompletedOrNull());
				Assert.AreEqual(3, buffer.Size);

				Thread.Sleep(255);
				tasks = buffer.PopCompletedOrNull();
				Assert.AreEqual(3, tasks.Count);
				Assert.AreEqual(task5, tasks[0]);
				Assert.AreEqual(task6, tasks[1]);
				Assert.AreEqual(task7, tasks[2]);
				Assert.AreEqual(0, buffer.Size);
				
				Assert.Null(buffer.PopCompletedOrNull());
				Assert.AreEqual(0, buffer.Size);
			}

			[Test]
			public void Test_StopResumeComputing()
			{
				var buffer = new WorkBuffer();
				buffer.PutTask(Guid.NewGuid(), TimeSpan.FromMilliseconds(25));
				Thread.Sleep(20);
				buffer.StopComputing();
				Thread.Sleep(20);
				Assert.Null(buffer.PopCompletedOrNull());
				Assert.AreEqual(1, buffer.Size);
				buffer.ResumeComputing();
				Thread.Sleep(6);
				Assert.AreEqual(1, buffer.PopCompletedOrNull().Count);
				Assert.AreEqual(0, buffer.Size);
			}

			[Test]
			public void Test_GetAvailabilityTime()
			{
				var buffer = new WorkBuffer();
				Thread.Sleep(100);
				buffer.StopComputing();
				Assert.AreEqual(TimeSpan.Zero, buffer.GetAvailabilityTime());
				buffer.PutTask(Guid.NewGuid(), TimeSpan.FromMilliseconds(25));
				Assert.AreEqual(TimeSpan.FromMilliseconds(25), buffer.GetAvailabilityTime());
				buffer.PutTask(Guid.NewGuid(), TimeSpan.FromMilliseconds(25));
				Assert.AreEqual(TimeSpan.FromMilliseconds(50), buffer.GetAvailabilityTime());

				buffer.ResumeComputing();
				Thread.Sleep(100);
				Assert.AreEqual(TimeSpan.Zero, buffer.GetAvailabilityTime());
				buffer.PopCompletedOrNull();
				Assert.AreEqual(TimeSpan.Zero, buffer.GetAvailabilityTime());
			}
		}
	}
}