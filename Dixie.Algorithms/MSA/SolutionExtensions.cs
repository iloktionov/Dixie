using System;
using System.Linq;

namespace Dixie.Core
{
	internal static class SolutionExtensions
	{
		public static Int32[] CloneSolution(this Int32[] solution)
		{
			var newSolution = new Int32[solution.Length];
			Array.Copy(solution, newSolution, solution.Length);
			return newSolution;
		}

		public static bool EqualsSolution(this Int32[] solution, Int32[] anotherSolution)
		{
			if (solution.Length != anotherSolution.Length)
				return false;
			return !solution.Where((t, i) => t != anotherSolution[i]).Any();
		}

		public static void PrintSolution(this Int32[] solution)
		{
			Console.Out.WriteLine(String.Join(" ", solution));
		}
	}
}