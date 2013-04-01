using System;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class AddNodesMutator_Tests
	{
		[Test]
		[Timeout(5 * 1000)]
		public void Test_CorrectWork()
		{
			var topology = Topology.CreateEmpty();
			var configurator = new TopologyConfigurator();
			var mutator = new AddNodesMutator(new Random(), configurator, 1000);
			topology.AddNode(new Node(1, 0), topology.MasterNode, TimeSpan.FromMilliseconds(1));

			while (topology.WorkerNodesCount < 1000)
				mutator.Mutate(topology);
			Assert.AreEqual(1000, topology.WorkerNodesCount);
		}
	}
}