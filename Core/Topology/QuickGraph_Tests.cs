using System;
using NUnit.Framework;
using QuickGraph;

namespace Dixie.Core
{
	[TestFixture]
	internal class QuickGraph_Tests
	{
		[Test]
		[Ignore]
		public void Test_RemoveFromGraph()
		{
			var graph = new BidirectionalGraph<INode, NetworkLink>();
			graph.AddVertex(node1);
			graph.AddVertex(node2);
			graph.AddVertex(node3);
			graph.AddVertex(node4);
			graph.AddVertex(node5);
			graph.AddEdge(new NetworkLink(node5, node4, TimeSpan.FromSeconds(1)));
			graph.AddEdge(new NetworkLink(node4, node3, TimeSpan.FromSeconds(1)));
			graph.AddEdge(new NetworkLink(node3, node2, TimeSpan.FromSeconds(1)));
			graph.AddEdge(new NetworkLink(node2, node1, TimeSpan.FromSeconds(1)));

			Console.Out.WriteLine("");
			Console.Out.WriteLine(String.Join("\n", graph.Edges));
			graph.RemoveVertex(node3);
			Console.Out.WriteLine("Removed node with id {0}..", node3.Id);
			Console.Out.WriteLine("");
			Console.Out.WriteLine(String.Join("\n", graph.Edges));
		}

		private readonly Node node1 = new Node(1.0d, 0.05d);
		private readonly Node node2 = new Node(2.0d, 0.05d);
		private readonly Node node3 = new Node(3.0d, 0.05d);
		private readonly Node node4 = new Node(4.0d, 0.05d);
		private readonly Node node5 = new Node(5.0d, 0.05d);
	}
}