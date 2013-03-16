using System;

namespace Dixie.Core
{
	[Serializable]
	public struct IntermediateTestResult
	{
		public IntermediateTestResult(Double workDone, TimeSpan timeElapsed)
		{
			WorkDone = workDone;
			TimeElapsed = timeElapsed;
		}

		public override string ToString()
		{
			return String.Format("{0}: {1:0.000}", TimeElapsed, WorkDone);
		}

		public Double WorkDone;
		public TimeSpan TimeElapsed;
	}
}