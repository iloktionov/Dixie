using System;
using System.Threading;

namespace Dixie.Core
{
	internal static class ThreadRunner
	{
		public static Thread Run(Action threadRoutine, Action<Thread> tuneThreadBeforeStart = null)
		{
			var t = new Thread(ThreadRoutineWrapper.Wrap(threadRoutine))
				{
					IsBackground = true
				};
			if (tuneThreadBeforeStart != null)
				tuneThreadBeforeStart(t);
			t.Start();
			return t;
		}

		public static Thread RunPeriodicAction(Action periodicAction, TimeSpan period, WaitHandle waitHandle)
		{
			return Run(() =>
			{
				waitHandle.WaitOne();
				while (true)
				{
					Thread.Sleep(period);
					periodicAction();
				}
			});
		}

		public static void StopThreads(params Thread[] threads)
		{
			foreach (Thread thread in threads)
				thread.Abort();
			foreach (Thread thread in threads)
				thread.Join();
		}
	}
}