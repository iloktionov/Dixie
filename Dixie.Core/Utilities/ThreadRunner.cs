using System;
using System.Threading;

namespace Dixie.Core
{
	public static class ThreadRunner
	{
		public static Thread Run(Action threadRoutine, Action<Thread> tuneThreadBeforeStart = null, Action<Exception> onException = null)
		{
			var thread = new Thread(ThreadRoutineWrapper.Wrap(threadRoutine, onException))
			{
				IsBackground = true
			};
			if (tuneThreadBeforeStart != null)
				tuneThreadBeforeStart(thread);
			thread.Start();
			return thread;
		}

		public static Thread RunPeriodicAction(Action periodicAction, TimeSpan period, Action<Thread> tuneThreadBeforeStart = null, WaitHandle waitHandle = null, Action<Exception> onException = null)
		{
			return Run(() =>
			{
				if (waitHandle != null)
					waitHandle.WaitOne();
				while (true)
				{
					Thread.Sleep(period);
					periodicAction();
				}
			}, tuneThreadBeforeStart, onException);
		}

		public static void StopThreads(params Thread[] threads)
		{
			foreach (Thread thread in threads)
				if (thread != null)
					thread.Abort();
			foreach (Thread thread in threads)
				if (thread != null)
					thread.Join();
		}
	}
}