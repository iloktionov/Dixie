﻿using System;
using System.Threading;

namespace Dixie.Core
{
	internal static class ThreadRunner
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

		public static Thread RunPeriodicAction(Action periodicAction, TimeSpan period, WaitHandle waitHandle, Action<Exception> onException = null)
		{
			return Run(() =>
			{
				waitHandle.WaitOne();
				while (true)
				{
					Thread.Sleep(period);
					periodicAction();
				}
			}, null, onException);
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