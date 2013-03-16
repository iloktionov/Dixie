using System;

namespace Dixie.Core
{
	internal static class CommutationAnySize
	{
		public static int[] GetRandomCommutation(int size)
		{
			return GetRandomCommutation(size, 0);
		}

		public static int[] GetRandomCommutation(int size, int randomFactor)
		{
			return GetRandomCommutation(size, randomFactor, DateTime.Now.Ticks);
		}

		private static int[] GetRandomCommutation(int size, int randomFactor, long timeSeed)
		{
			if (size == 1) 
				return new[] { 0 };

			var random = new Random(unchecked((int)timeSeed + randomFactor));
			var commArray = new int[size];
			for (int i = 0; i < size; i++)
				commArray[i] = i;

			for (int i = 0; i < size - 1; i++)
			{
				int rnd = random.Next(size - i);
				Swap(ref commArray[i], ref commArray[i + rnd]);
			}

			return commArray;
		}

		private static void Swap(ref int i1, ref int i2)
		{
			int tmp = i1;
			i1 = i2;
			i2 = tmp;
		}
	}
}