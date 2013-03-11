﻿using System;
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
			

			Assert.AreEqual(task1, buffer.PopCompletedOrNull().Single());
			Assert.AreEqual(3, buffer.Size);

			Thread.Sleep(TimeSpan.FromMilliseconds(55));
			Assert.AreEqual(task2, buffer.PopCompletedOrNull().Single());
			Assert.AreEqual(2, buffer.Size);

			Thread.Sleep(TimeSpan.FromMilliseconds(105));
			Assert.AreEqual(task3, buffer.PopCompletedOrNull().Single());
			Assert.AreEqual(1, buffer.Size);

			Thread.Sleep(TimeSpan.FromMilliseconds(105));
			Assert.AreEqual(task4, buffer.PopCompletedOrNull().Single());
			Assert.AreEqual(0, buffer.Size);

			Thread.Sleep(TimeSpan.FromMilliseconds(50));
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
	}
}