using System;

namespace Dixie.Core
{
	internal static class NodeFailureHelper
	{
		public static TimeSpan GetFailureTime(NodeFailureType failureType)
		{
			if (failureType == NodeFailureType.ShortTerm)
				return TimeSpan.FromSeconds(3);
			if (failureType == NodeFailureType.LongTerm)
				return TimeSpan.FromMinutes(1);
			return TimeSpan.Zero;
		}
	}
}