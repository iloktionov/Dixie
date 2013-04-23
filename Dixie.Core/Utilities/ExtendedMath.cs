using System;

namespace Dixie.Core
{
	public static class ExtendedMath
	{
		public static TimeSpan Min(TimeSpan t1, TimeSpan t2)
		{
			return TimeSpan.FromTicks(Math.Min(t1.Ticks, t2.Ticks));
		}

		public static TimeSpan Max(TimeSpan t1, TimeSpan t2)
		{
			return TimeSpan.FromTicks(Math.Max(t1.Ticks, t2.Ticks));
		}

		public static Double Max(Double d1, Double d2, Double d3)
		{
			return Math.Max(d1, Math.Max(d2, d3));
		}
	}
}