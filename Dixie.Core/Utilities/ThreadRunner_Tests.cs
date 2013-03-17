using System.Threading;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class ThreadRunner_Tests
	{
		[Test]
		[Timeout(5000)]
		public void Test_StopThreads()
		{
			var thread = ThreadRunner.Run(() => Thread.Sleep(Timeout.Infinite));
			ThreadRunner.StopThreads(thread);
			ThreadRunner.StopThreads(thread);
			ThreadRunner.StopThreads(thread);
		}
	}
}