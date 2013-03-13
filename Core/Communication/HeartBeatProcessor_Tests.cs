using System;
using System.Collections.Generic;
using System.Threading;
using NUnit.Framework;

namespace Dixie.Core
{
	internal partial class HeartBeatProcessor
	{
		[TestFixture]
		internal class HeartBeatProcessor_Tests
		{
			[Test]
			public void Test_DeliverMessagesAndResponses()
			{
				var master = new Master();
				var topology = Topology.CreateEmpty();
				var node1 = new Node(100, 0.2);
				var node2 = new Node(100, 0.2);
				Console.Out.WriteLine("Node1 = {0}", node1.Id);
				Console.Out.WriteLine("Node2 = {0}", node2.Id);
				topology.AddNode(node1, topology.MasterNode, TimeSpan.FromMilliseconds(50));
				topology.AddNode(node2, topology.MasterNode, TimeSpan.FromMilliseconds(150));
				var task1 = new Task(0);
				var task2 = new Task(0);
				master.TaskManager.PutTasks(new[] { task1, task2 });
				master.TaskManager.AssignNodeToTask(task1, node1.Id);
				master.TaskManager.AssignNodeToTask(task2, node2.Id);

				var processor = new HeartBeatProcessor(topology, master, TimeSpan.FromSeconds(1));
				var processorThread = new Thread(() =>
				{
					while (true)
					{
						processor.DeliverMessagesAndResponses();
						Thread.Sleep(1);
					}
				});
				processorThread.Start();

				try
				{
					Thread.Sleep(5);
					PrintProcessorState(processor);
					Assert.AreEqual(2, processor.outgoingMessages.Count);

					Thread.Sleep(65);
					PrintProcessorState(processor);
					Assert.AreEqual(1, processor.outgoingMessages.Count);
					Assert.AreEqual(1, processor.incomingResponses.Count);

					Thread.Sleep(350);
					PrintProcessorState(processor);
					Assert.AreEqual(0, processor.outgoingMessages.Count);
					Assert.AreEqual(0, processor.incomingResponses.Count);

					Assert.AreEqual(1, node1.GetHeartBeatMessage().CompletedTasks.Count);
					Assert.AreEqual(1, node2.GetHeartBeatMessage().CompletedTasks.Count);
				}
				finally
				{
					processorThread.Abort();
					processorThread.Join();
				}
			}

			[Test]
			public void Test_NeededToSendMessage()
			{
				var node = new Node(100, 0.2);
				var processor = new HeartBeatProcessor(null, null, TimeSpan.FromMilliseconds(100));
				Assert.True(processor.NeededToSendMessage(node, processor.watch.Elapsed));
				Assert.False(processor.NeededToSendMessage(node, processor.watch.Elapsed));
				Thread.Sleep(50);
				Assert.False(processor.NeededToSendMessage(node, processor.watch.Elapsed));
				Thread.Sleep(55);
				Assert.True(processor.NeededToSendMessage(node, processor.watch.Elapsed));
				Assert.False(processor.NeededToSendMessage(node, processor.watch.Elapsed));
			}

			private static void PrintProcessorState(HeartBeatProcessor processor)
			{
				Console.Out.WriteLine("");
				Console.Out.WriteLine("Time = {0}", processor.watch.Elapsed);
				Console.Out.WriteLine("Outgoing messages:");
				foreach (KeyValuePair<HeartBeatMessage, TimeSpan> pair in processor.outgoingMessages)
					Console.Out.WriteLine("\tNode = {0}; Delivery time = {1}", pair.Key.NodeId, pair.Value);
				Console.Out.WriteLine("Incoming responses:");
				foreach (KeyValuePair<HeartBeatResponse, TimeSpan> pair in processor.incomingResponses)
					Console.Out.WriteLine("\tNode = {0}; Delivery time = {1}", pair.Key.NodeId, pair.Value);
			}
		}
	}
}