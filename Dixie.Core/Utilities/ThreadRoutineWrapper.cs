using System;
using System.Threading;

namespace Dixie.Core
{
	internal static class ThreadRoutineWrapper
	{
		public static ThreadStart Wrap(Action threadRoutine, Action<Exception> onException = null)
		{
			return () =>
				{
					try
					{
						threadRoutine();
					}
					catch (ThreadAbortException)
					{
						Thread.ResetAbort();
					}
					catch (Exception error)
					{
						if (onException != null)
							ThreadPool.QueueUserWorkItem(obj => onException(error));
						else throw;
					}
				};
		}
	}
}