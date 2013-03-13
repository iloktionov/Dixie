using System;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class NodeFailureProbabilities_Tests
	{
		[Test]
		public void Test_Generate()
		{
			var random = new Random();
			for (int i = 0; i < 10; i++)
				Console.Out.WriteLine(NodeFailureProbabilities.Generate(random));
		}

		[Test]
		public void Test_DetermineFailureType_1()
		{
			var prob = new NodeFailureProbabilities(0, 0, 1);
			var random = new Random();
			for (int i = 0; i < 1000; i++)
				Assert.AreEqual(NodeFailureType.Permanent, prob.DetermineFailureType(random));
		}

		[Test]
		public void Test_DetermineFailureType_2()
		{
			var prob = new NodeFailureProbabilities(0, 1, 0);
			var random = new Random();
			for (int i = 0; i < 1000; i++)
				Assert.AreEqual(NodeFailureType.LongTerm, prob.DetermineFailureType(random));
		}
	}
}