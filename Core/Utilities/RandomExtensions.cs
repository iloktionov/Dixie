using System;

namespace Dixie.Core
{
	public static class RandomExtensions
	{
		public static bool FlipCoin(this Random random)
		{
			return random.NextDouble() <= 0.5d;
		}

		public static bool TemptProvidence(this Random random, double probability)
		{
			return random.NextDouble() <= probability;
		}
	}
}