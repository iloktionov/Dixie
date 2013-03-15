using System;
using System.Threading;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class GarbageCollector_Tests
	{
		[Test]
		public void  Test_CorrectWork()
		{
			var gc = new GarbageCollector(TimeSpan.Zero);
			gc.AddStaleNode(Guid.NewGuid());
			gc.AddStaleNode(Guid.NewGuid());
			gc.AddStaleNode(Guid.NewGuid());
			Assert.AreEqual(3, gc.Count);
			
			gc.CollectGarbage(new Master(TimeSpan.Zero, new ColorConsoleLog()));
			Assert.AreEqual(0, gc.Count);
		}

		[Test]
		public void Test_Delay()
		{
			var gc = new GarbageCollector(TimeSpan.FromMilliseconds(5));
			gc.AddStaleNode(Guid.NewGuid());
			Thread.Sleep(15);
			gc.AddStaleNode(Guid.NewGuid());
			gc.AddStaleNode(Guid.NewGuid());
			Thread.Sleep(10);

			gc.CollectGarbage(new Master(TimeSpan.Zero, new ColorConsoleLog()));
			Assert.AreEqual(2, gc.Count);
		}
	}
}