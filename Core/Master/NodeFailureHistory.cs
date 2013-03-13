using System;
using System.Collections.Generic;
using System.Linq;

namespace Dixie.Core
{
	public class NodeFailureHistory
	{
		public NodeFailureHistory()
		{
			FailureDurations = new List<TimeSpan>();
		}

		public bool HasFailures()
		{
			return FailureDurations.Any();
		}

		public TimeSpan MaxFailureTime()
		{
			return FailureDurations.Max();
		}

		public TimeSpan MinFailureTime()
		{
			return FailureDurations.Min();
		}

		internal void AddFailure(TimeSpan failureDuration)
		{
			FailureDurations.Add(failureDuration);
		}

		public List<TimeSpan> FailureDurations { get; private set; }
	}
}