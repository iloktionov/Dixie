using System;
using System.Collections.Generic;
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
			var state = new InitialGridState(topology, random.Next(), TopologySettings.GetInstance(), EngineSettings.GetInstance());
			var engine = new Engine(state, new ColorConsoleLog());
			ISchedulerAlgorithm algorithm1 = new RandomAlgorithm(new Random(123), "Random1");
			ISchedulerAlgorithm algorithm2 = new RandomAlgorithm(new Random(123), "Random2");

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

		[Test]
		public void Test_CatchExceptionsInTestThreads()
		{
			var topology = new TopologyBuilder().Build(500);
			var state = new InitialGridState(topology, random.Next(), TopologySettings.GetInstance(), EngineSettings.GetInstance());
			var engine = new Engine(state, new ColorConsoleLog());
			ISchedulerAlgorithm algorithm = new ErrorAlgorithm();

			try
			{
				engine.TestAlgorithm(algorithm, TimeSpan.FromSeconds(1), TimeSpan.FromMilliseconds(10));
				Assert.Fail("Engine must throw EngineException.");
			}
			catch (EngineException error)
			{
				Console.Out.WriteLine(error);
				Assert.AreEqual(0, engine.GetRunningThreadsCount());
			}
			catch (Exception error)
			{
				Console.Out.WriteLine(error);
				Assert.Fail("Engine must throw EngineException.");
			}
		}

		[Test]
		public void Test_Stop()
		{
			var topology = new TopologyBuilder().Build(500);
			var state = new InitialGridState(topology, random.Next(), TopologySettings.GetInstance(), EngineSettings.GetInstance());
			var engine = new Engine(state, new ColorConsoleLog());
			engine.Stop();
		}

		private class ErrorAlgorithm : ISchedulerAlgorithm
		{
			public void Work(List<NodeInfo> aliveNodes, TaskManager taskManager)
			{
				throw new Exception("Test exception from ErrorAlgorithm!");
			}

			public void Reset() { }

			public string Name { get { return "Error"; } set {} }
		}

		private readonly Random random = new Random();
	}
}