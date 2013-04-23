using System;
using System.Linq;
using NUnit.Framework;

namespace Dixie.Core
{
	internal partial class InitialMutationsProcessor
	{
		[TestFixture]
		internal class InitialMutationsProcessor_Tests
		{
			[Test]
			public void Test_MutateAndRollback()
			{
				const int tasks = 5;
				const int nodes = 3;
				var etcMatrix = GenerateETCMatrix(tasks, nodes);
				var vector = GenerateAvailabilityVector(nodes);
				var solution = new[] {0, 1, 2, 1, 1};
				var calculator = new InitialMutationsProcessor(solution, new Random(), etcMatrix, vector);

				solution.PrintSolution();
				SingleExchangeMutation mutation = calculator.Mutate();
				calculator.initialSolution.PrintSolution();
				calculator.Rollback(mutation);
				calculator.initialSolution.PrintSolution();
				Assert.True(solution.EqualsSolution(calculator.initialSolution));
			}

			[Test]
			public void Test_GetMakespan()
			{
				const int tasks = 5;
				const int nodes = 3;
				var etcMatrix = GenerateETCMatrix(tasks, nodes);
				var vector = GenerateAvailabilityVector(nodes);
				var solution = new[] { 2, 1, 2, 1, 0 };
				var calculator = new InitialMutationsProcessor(solution, new Random(123), etcMatrix, vector);

				solution.PrintSolution();
				PrintCompletionTimes(calculator);
				SingleExchangeMutation mutation = calculator.Mutate();
				calculator.GetMakespan(mutation);
				calculator.initialSolution.PrintSolution();
				PrintCompletionTimes(calculator);
				calculator.Rollback(mutation);
				PrintCompletionTimes(calculator);

				for (int i = 0; i < 100; i++)
				{
					SingleExchangeMutation mutation2 = calculator.Mutate();
					Double makespan = calculator.GetMakespan(mutation2);
					PrintCompletionTimes(calculator);
					Console.Out.WriteLine(makespan);
					calculator.Rollback(mutation2);
				}
				PrintCompletionTimes(calculator);
			}

			private static void PrintCompletionTimes(InitialMutationsProcessor processor)
			{
				Console.Out.WriteLine(String.Join(" ", processor.completionTimes.Select(d => String.Format("{0:0.000}", d))));
			}

			private static Double[] GenerateAvailabilityVector(int nodes)
			{
				return new Double[nodes];
			}

			private static Double[,] GenerateETCMatrix(int tasks, int nodes)
			{
				var result = new Double[tasks,nodes];
				for (int i = 0; i < tasks; i++)
					for (int j = 0; j < nodes; j++)
						result[i, j] = i + 1;
				return result;
			}

			private static readonly Random random = new Random();
		}
	}
}