using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

namespace Dixie.Core
{
	[TestFixture]
	internal class MinMinAlgorithm_Tests
	{
		[Test]
		[Ignore]
		public void Test_Speed()
		{
			List<Task> tasks = GenerateTasks(1 * 1000);
			List<NodeInfo> nodes = GenerateNodes(500);
			var algorithm = new MinMinAlgorithm();
			var watch = Stopwatch.StartNew();
			algorithm.AssignNodes(nodes, tasks);
			Console.Out.WriteLine(watch.Elapsed);
		}

		private List<Task> GenerateTasks(int count)
		{
			var result = new List<Task>(count);
			for (int i = 0; i < count; i++)
				result.Add(new Task(random.NextDouble() * 100000));
			return result;
		}

		private List<NodeInfo> GenerateNodes(int count)
		{
			var result = new List<NodeInfo>(count);
			for (int i = 0; i < count; i++)
				result.Add(new NodeInfo(Guid.NewGuid(), random.NextDouble()*100, TimeSpan.Zero, 0, TimeSpan.MinValue, TimeSpan.Zero));
			return result;
		}

		private readonly Random random = new Random();
	}
}