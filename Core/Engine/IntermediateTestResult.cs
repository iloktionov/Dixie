using System;

namespace Dixie.Core
{
	[Serializable]
	internal struct IntermediateTestResult
	{
		public IntermediateTestResult(Double workDone, TimeSpan timeElapsed)
		{
			WorkDone = workDone;
			TimeElapsed = timeElapsed;
		}

		public Double WorkDone;
		public TimeSpan TimeElapsed;
	}
}