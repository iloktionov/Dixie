using System;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class Engine_Tests
	{
		[Test]
		[Ignore]
		public void Test_AlgorithmTesting()
		{
			var topology = new TopologyBuilder().Build(500);
			var state = new InitialGridState(topology, 3456546, TopologySettings.GetInstance(), EngineSettings.GetInstance());
			var engine = new Engine(state);

			var result = engine.TestAlgorithm(new RandomAlgorithm(), TimeSpan.FromSeconds(0.5));
			Console.Out.WriteLine(result.TotalWorkDone);
		}
	}
}