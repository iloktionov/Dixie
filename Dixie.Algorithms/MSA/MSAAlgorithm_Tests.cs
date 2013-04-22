using System;
using System.Collections.Generic;
using System.Diagnostics;
using NUnit.Framework;

namespace Dixie.Core
{
	internal partial class MSAAlgorithm
	{
		[TestFixture]
		internal class MSAAlgorithm_Tests
		{
			[Test]
			[Ignore]
			public void Test_Speed()
			{
				var aliveNodes = new List<NodeInfo>(1000);
				var tasks = new List<Task>(3000);
				var random = new Random();
				for (int i = 0; i < 1000; i++)
					aliveNodes.Add(new NodeInfo(Guid.NewGuid(), random.Next(50, 100), TimeSpan.Zero, 0, TimeSpan.Zero, TimeSpan.Zero));
				for (int i = 0; i < 3000; i++)
					tasks.Add(new Task(random.Next(500, 100000)));

				var algorithm = new MSAAlgorithm(new ColorConsoleLog());
				var watch = Stopwatch.StartNew();
				algorithm.AssignNodes(aliveNodes, tasks);
				Console.Out.WriteLine(watch.Elapsed);
			}
		}
	}
}