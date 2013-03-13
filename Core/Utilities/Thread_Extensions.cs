using System.Threading;

namespace Dixie.Core
{
	internal static class Thread_Extensions
	{
		public static void AbortAndWaitCompleted(this Thread thread)
		{
			if (thread == null)
				return;
			thread.Abort();
			thread.Join();
		}
	}
}