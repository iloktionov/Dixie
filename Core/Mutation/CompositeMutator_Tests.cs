using System;
using System.Diagnostics;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class CompositeMutator_Tests
	{
		[Test]
		// (iloktionov): Проверяем, что мутатор ни от чего не падает при долгой работе. 
		public void Test_CorrectWork()
		{
			Topology topology = GenerateInitialTopology(1000);
			var mutator = new CompositeMutator(24234, 1000, 0.1, 0.1);
			const int MutationsCount = 10 * 1000;
			var watch = Stopwatch.StartNew();
			for (int i = 0; i < MutationsCount; i++)
			{
				mutator.Mutate(topology);
				if (i % 500 == 0)
					Console.Out.WriteLine("Remaining nodes = {0}", topology.WorkerNodesCount);
			}
			Console.Out.WriteLine("Did {0} mutations in {1}.", MutationsCount, watch.Elapsed);
		}

		private Topology GenerateInitialTopology(int nodesCount)
		{
			return new TopologyBuilder().Build(nodesCount);
		}
	}
}