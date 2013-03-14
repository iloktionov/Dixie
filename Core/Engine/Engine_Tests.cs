using System;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class Engine_Tests
	{
		[Test]
		public void Test_AlgorithmTesting()
		{
			var topology = new TopologyBuilder().Build(500);
			var state = new InitialGridState(topology, 3456546, TopologySettings.GetInstance(), EngineSettings.GetInstance());
			var engine = new Engine(state, new ColorConsoleLog());
			ISchedulerAlgorithm algorithm1 = new RandomAlgorithm(new Random(123));
			ISchedulerAlgorithm algorithm2 = new RandomAlgorithm(new Random(123));

			var result1 = engine.TestAlgorithm(algorithm1, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(50));
			var result2 = engine.TestAlgorithm(algorithm2, TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(50));
			Console.Out.WriteLine(result1.TotalWorkDone);
			Console.Out.WriteLine(result2.TotalWorkDone);
			Console.Out.WriteLine(result2.IntermediateResults.Count);
			Console.Out.WriteLine(result2.IntermediateResults.Count);

			// (iloktionov): Т.к. у обоих алгоритмов одинаковый seed, они должны показать очень близкие результаты.
			double differenceInPct = Math.Abs(1 - result1.TotalWorkDone/result2.TotalWorkDone) * 100d;
			Console.Out.WriteLine("Difference pct = {0:0.000}%", differenceInPct);
			Assert.Less(differenceInPct, 5);
		}
	}
}