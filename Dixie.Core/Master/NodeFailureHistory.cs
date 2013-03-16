using System;
using System.Collections.Generic;
using System.Linq;

namespace Dixie.Core
{
	public class NodeFailureHistory
	{
		public NodeFailureHistory()
		{
			Failures = new List<NodeFailure>();
		}

		public bool HasFailures()
		{
			return Failures.Any();
		}

		public TimeSpan MaxFailureTime()
		{
			return Failures.Max(failure => failure.Duration);
		}

		public TimeSpan MinFailureTime()
		{
			return Failures.Min(failure => failure.Duration);
		}

		internal void AddFailure(TimeSpan detectionTime, TimeSpan duration)
		{
			Failures.Add(new NodeFailure(detectionTime, duration));
		}

		public List<NodeFailure> Failures{ get; private set; }
	}
}