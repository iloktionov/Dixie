using System;

namespace Dixie.Core
{
	public struct NodeFailure
	{
		public NodeFailure(TimeSpan detectionTime, TimeSpan duration)
		{
			DetectionTime = detectionTime;
			Duration = duration;
		}

		public TimeSpan DetectionTime;
		public TimeSpan Duration;
	}
}