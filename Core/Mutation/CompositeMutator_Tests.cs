using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
			var mutator = new CompositeMutator(24234, 1000, 0.1, 0.1, TopologySettings.GetInstance());
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

		[Test]
		// (iloktionov): Проверяем, что два разных мутатора с одинаковым seed дадут две эквивалентные топологии на выходе.
		public void Test_Determinism()
		{
			int seed = new Random().Next();
			var initialTopology = GenerateInitialTopology(1000);
			var newTopologyList = new List<Topology>();
			for (int i = 0; i < 5; i++)
			{
				Topology newTopology = initialTopology.Clone();
				MutateTopology(newTopology, 1250, seed);
				newTopologyList.Add(newTopology);
			}
			Console.Out.WriteLine("New topologies: {0}", newTopologyList.Count);
			for (int i = 0; i < newTopologyList.Count - 1; i++)
				CompareTopologies(newTopologyList[i], newTopologyList[i + 1]);
		}

		private static Topology GenerateInitialTopology(int nodesCount)
		{
			return new TopologyBuilder().Build(nodesCount);
		}

		private static void MutateTopology(Topology topology, int mutationsCount, int seed)
		{
			var mutator = new CompositeMutator(24234, 1000, 0.1, 0.1, TopologySettings.GetInstance());
			for (int i = 0; i < mutationsCount; i++)
				mutator.Mutate(topology);
		}

		private static void CompareTopologies(Topology topology1, Topology topology2)
		{
			Console.Out.WriteLine("Comparing topology");
			Assert.AreEqual(topology1.WorkerNodesCount, topology2.WorkerNodesCount);
			Assert.AreEqual(topology1.Graph.VertexCount, topology2.Graph.VertexCount);
			Assert.AreEqual(topology1.Graph.EdgeCount, topology2.Graph.EdgeCount);
			Assert.AreEqual(topology1.WorkerLatencies.Count, topology2.WorkerLatencies.Count);
			List<TimeSpan> latencies1 = topology1.WorkerLatencies.Values.ToList();
			List<TimeSpan> latencies2 = topology2.WorkerLatencies.Values.ToList();
			for (int i = 0; i < latencies1.Count; i++)
				Assert.AreEqual(latencies1[i], latencies2[i]);
		}
	}
}