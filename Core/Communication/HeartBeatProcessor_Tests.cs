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
				var master = new Master(TimeSpan.Zero, new ColorConsoleLog());
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

			[Test]
			public void Test_SendMessages()
			{
				Topology topology = Topology.CreateEmpty();
				var master = new Master(TimeSpan.MaxValue, new ColorConsoleLog());
				var node1 = new Node(1, 0.1d);
				var node2 = new Node(2, 0.2d);
				var node3 = new Node(3, 0.3d);
				topology.AddNode(node1, topology.MasterNode, TimeSpan.FromMilliseconds(1));
				topology.AddNode(node2, topology.MasterNode, TimeSpan.FromMilliseconds(2));
				topology.AddNode(node3, topology.MasterNode, TimeSpan.FromMilliseconds(3));
				var processor = new HeartBeatProcessor(topology, master, TimeSpan.FromMilliseconds(5));

				processor.SendMessages(new [] {node1, node2, node3, new Node(4, 0.4d) });
				Assert.AreEqual(3, processor.outgoingMessages.Count);
				Assert.AreEqual(node1.Id, processor.outgoingMessages[0].Key.NodeId);
				Assert.AreEqual(node2.Id, processor.outgoingMessages[1].Key.NodeId);
				Assert.AreEqual(node3.Id, processor.outgoingMessages[2].Key.NodeId);
				Assert.AreEqual(TimeSpan.FromMilliseconds(1), processor.outgoingMessages[0].Key.CommunicationLatency);
				Assert.AreEqual(TimeSpan.FromMilliseconds(2), processor.outgoingMessages[1].Key.CommunicationLatency);
				Assert.AreEqual(TimeSpan.FromMilliseconds(3), processor.outgoingMessages[2].Key.CommunicationLatency);
				Assert.AreEqual(TimeSpan.FromMilliseconds(1), processor.outgoingMessages[1].Value - processor.outgoingMessages[0].Value);
				Assert.AreEqual(TimeSpan.FromMilliseconds(1), processor.outgoingMessages[2].Value - processor.outgoingMessages[1].Value);
			}

			[Test]
			public void Test_DeliverOutgoingMessages()
			{
				Topology topology = Topology.CreateEmpty();
				var master = new Master(TimeSpan.MaxValue, new ColorConsoleLog());
				var node1 = new Node(1, 0.1d);
				var node2 = new Node(2, 0.2d);
				var node3 = new Node(3, 0.3d);
				var processor = new HeartBeatProcessor(topology, master, TimeSpan.FromMilliseconds(5));

				var message1 = new HeartBeatMessage(node1.Id, 0) {CommunicationLatency = TimeSpan.FromMilliseconds(1)};
				var message2 = new HeartBeatMessage(node2.Id, 0) {CommunicationLatency = TimeSpan.FromMilliseconds(2)};
				var message3 = new HeartBeatMessage(node3.Id, 0) {CommunicationLatency = TimeSpan.FromMilliseconds(3)};
				processor.outgoingMessages.Add(new KeyValuePair<HeartBeatMessage, TimeSpan>(message1, TimeSpan.MinValue));
				processor.outgoingMessages.Add(new KeyValuePair<HeartBeatMessage, TimeSpan>(message2, TimeSpan.MinValue));
				processor.outgoingMessages.Add(new KeyValuePair<HeartBeatMessage, TimeSpan>(message3, TimeSpan.MaxValue));

				processor.DeliverOutgoingMessages();
				Assert.AreEqual(1, processor.outgoingMessages.Count);
				Assert.AreEqual(2, processor.incomingResponses.Count);
				Assert.AreEqual(TimeSpan.FromMilliseconds(1), processor.incomingResponses[1].Value - processor.incomingResponses[0].Value);
			}

			[Test]
			public void Test_DeliverIncomingResponse()
			{
				Topology topology = Topology.CreateEmpty();
				var master = new Master(TimeSpan.MaxValue, new ColorConsoleLog());
				var node1 = new Node(1, 0.1d);
				var node2 = new Node(2, 0.2d);
				var node3 = new Node(3, 0.3d);
				topology.AddNode(node1, topology.MasterNode, TimeSpan.FromMilliseconds(1));
				topology.AddNode(node2, topology.MasterNode, TimeSpan.FromMilliseconds(2));
				topology.AddNode(node3, topology.MasterNode, TimeSpan.FromMilliseconds(3));
				var processor = new HeartBeatProcessor(topology, master, TimeSpan.FromMilliseconds(5));

				var task = new Task(1000d);
				processor.incomingResponses.Add(new KeyValuePair<HeartBeatResponse, TimeSpan>(new HeartBeatResponse(node1.Id), TimeSpan.MaxValue));
				processor.incomingResponses.Add(new KeyValuePair<HeartBeatResponse, TimeSpan>(new HeartBeatResponse(node2.Id), TimeSpan.MinValue));
				processor.incomingResponses.Add(new KeyValuePair<HeartBeatResponse, TimeSpan>(new HeartBeatResponse(node3.Id, new List<Task>{task}), TimeSpan.MinValue));
				processor.incomingResponses.Add(new KeyValuePair<HeartBeatResponse, TimeSpan>(new HeartBeatResponse(Guid.NewGuid()), TimeSpan.MinValue));

				processor.DeliverIncomingResponses();
				Assert.AreEqual(1, processor.incomingResponses.Count);
				Assert.AreEqual(node1.Id, processor.incomingResponses[0].Key.NodeId);
				Assert.AreEqual(1, node3.GetHeartBeatMessage().WorkBufferSize);
				
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