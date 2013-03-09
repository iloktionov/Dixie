using System;
using NUnit.Framework;
using QuickGraph.Algorithms.ConnectedComponents;

namespace Dixie.Core
{
	[TestFixture]
	internal class TopologyBuilder_Tests
	{
		[Test]
		public void Test_CorrectWork()
		{
			var builder = new TopologyBuilder(3, 20);
			Topology topology = builder.Build(1000);
			Assert.AreEqual(1001, topology.Graph.VertexCount);
			Assert.AreEqual(1000, topology.Graph.EdgeCount);
			Assert.AreEqual(1000, topology.GetWorkerNodes().Count);
			var algo = new WeaklyConnectedComponentsAlgorithm<INode, NetworkLink>(topology.Graph);
			algo.Compute();
			Assert.AreEqual(1, algo.ComponentCount);
		}
	}
}