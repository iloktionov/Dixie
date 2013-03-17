using System;

namespace Dixie.Core
{
	public static class AlgorithmNamesHelper
	{
		public static void PrepareAlgorithmName(ISchedulerAlgorithm algorithm)
		{
			int index = algorithm.Name.LastIndexOf("#", StringComparison.InvariantCulture);
			if (index < 0)
				algorithm.Name += "#1";
			else
			{

				int prevNumber = Int32.Parse(algorithm.Name.Substring(index + 1));
				algorithm.Name = algorithm.Name.Substring(0, index) + "#" + (prevNumber + 1);
			}
		}

		public static void RestoreAlgorithmName(ISchedulerAlgorithm algorithm)
		{
			int index = algorithm.Name.LastIndexOf("#", StringComparison.InvariantCulture);
			if (index > 0)
				algorithm.Name = algorithm.Name.Substring(0, index);
		}
	}
}