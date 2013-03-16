using System;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class NodeFailurePattern_Tests
	{
		[Test]
		public void Test_Generate()
		{
			var random = new Random();
			for (int i = 0; i < 10; i++)
				Console.Out.WriteLine(NodeFailurePattern.Generate(random));
		}

		[Test]
		public void Test_DetermineFailureType_1()
		{
			var pattern = new NodeFailurePattern(0, 0, 1);
			var random = new Random();
			for (int i = 0; i < 1000; i++)
				Assert.AreEqual(NodeFailureType.Permanent, pattern.DetermineFailureType(random));
		}

		[Test]
		public void Test_DetermineFailureType_2()
		{
			var pattern = new NodeFailurePattern(0, 1, 0);
			var random = new Random();
			for (int i = 0; i < 1000; i++)
				Assert.AreEqual(NodeFailureType.LongTerm, pattern.DetermineFailureType(random));
		}
	}
}