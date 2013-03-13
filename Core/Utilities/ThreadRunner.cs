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

		public static Thread RunPeriodicAction(Action periodicAction, TimeSpan period, Action<Thread> tuneThreadBeforeStart = null)
		{
			return Run(() =>
			{
				while (true)
				{
					Thread.Sleep(period);
					periodicAction();
				}
			}, tuneThreadBeforeStart);
		}
 
		public static Thread Run(Action<object> threadRoutine, object threadRoutineParameter, Action<Thread> tuneThreadBeforeStart = null)
		{
			var t = new Thread(ThreadRoutineWrapper.Wrap(threadRoutine))
				{
					IsBackground = true
				};
			if (tuneThreadBeforeStart != null)
				tuneThreadBeforeStart(t);
			t.Start(threadRoutineParameter);
			return t;
		}
	}
}