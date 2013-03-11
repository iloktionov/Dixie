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
			var configurator = new TopologyConfigurator(
				new Range<int>(1, 2),
				new Range<double>(1, 2),
				new Range<double>(0, 1),
				new Range<TimeSpan>(TimeSpan.FromMilliseconds(1), TimeSpan.FromMilliseconds(10))
			);
			var mutator = new AddNodesMutator(new Random(), configurator, 1000);

			while (topology.WorkerNodesCount < 1000)
				mutator.Mutate(topology);
			Assert.AreEqual(1000, topology.WorkerNodesCount);
		}
	}
}