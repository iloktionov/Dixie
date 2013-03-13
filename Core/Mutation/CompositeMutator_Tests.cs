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
			for (int i = 0; i < 10 * 1000; i++)
				mutator.Mutate(topology);
		}

		private Topology GenerateInitialTopology(int nodesCount)
		{
			return new TopologyBuilder().Build(nodesCount);
		}
	}
}