﻿using System;

namespace Dixie.Core
{
	internal static class ExtendedMath
	{
		public static TimeSpan Min(TimeSpan t1, TimeSpan t2)
		{
			return TimeSpan.FromTicks(Math.Min(t1.Ticks, t2.Ticks));
		}

		public static TimeSpan Max(TimeSpan t1, TimeSpan t2)
		{
			return TimeSpan.FromTicks(Math.Max(t1.Ticks, t2.Ticks));
		}
	}
}