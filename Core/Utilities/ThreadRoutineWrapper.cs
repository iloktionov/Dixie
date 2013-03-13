using System;
using System.Threading;

namespace Dixie.Core
{
	internal static class ThreadRoutineWrapper
	{
		public static ParameterizedThreadStart Wrap(Action<object> threadRoutine)
		{
			return parameter =>
				{
					try
					{
						threadRoutine(parameter);
					}
					catch (ThreadAbortException)
					{
						Console.Out.WriteLine("Thread was aborted. ThreadRoutine: {0}", TryGetActionDescription(threadRoutine));
						Thread.ResetAbort();
					}
					catch (Exception error)
					{
						Console.Out.WriteLine("Unhandled error. ThreadRoutine: {0}. Exception: {1}", TryGetActionDescription(threadRoutine), error);
						throw;
					}
				};
		}

		public static ThreadStart Wrap(Action threadRoutine)
		{
			return () =>
				{
					try
					{
						threadRoutine();
					}
					catch (ThreadAbortException)
					{
						Console.Out.WriteLine("Thread was aborted. ThreadRoutine: {0}", TryGetActionDescription(threadRoutine));
						Thread.ResetAbort();
					}
					catch (Exception error)
					{
						Console.Out.WriteLine("Unhandled error. ThreadRoutine: {0}. Exception: {1}", TryGetActionDescription(threadRoutine), error);
						throw;
					}
				};
		}

		public static string TryGetActionDescription(Action<object> action)
		{
			try
			{
				return string.Format("method: {0}, declaringType: {1}", action.Method, action.Method.DeclaringType);
			}
			catch (Exception)
			{
				return "";
			}
		}

		public static string TryGetActionDescription(Action action)
		{
			try
			{
				return string.Format("method: {0}, declaringType: {1}", action.Method, action.Method.DeclaringType);
			}
			catch (Exception)
			{
				return "";
			}
		}
	}
}