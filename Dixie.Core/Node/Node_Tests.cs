using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using NUnit.Framework;

namespace Dixie.Core
{
	internal partial class Node
	{
		[TestFixture]
		internal class Node_Tests
		{
			[Test]
			public void Test_GetHeartBeatMessage()
			{
				var node = new Node(0.1, 0.1);
				Guid task1 = Guid.NewGuid();
				Guid task2 = Guid.NewGuid();
				node.workBuffer.PutTask(task1, TimeSpan.Zero);
				node.workBuffer.PutTask(task2, TimeSpan.FromMilliseconds(25));

				HeartBeatMessage hbm1 = node.GetHeartBeatMessage();
				Thread.Sleep(TimeSpan.FromMilliseconds(50));
				HeartBeatMessage hbm2 = node.GetHeartBeatMessage();
				HeartBeatMessage hbm3 = node.GetHeartBeatMessage();

				Assert.AreEqual(node.Id, hbm1.NodeId);
				Assert.AreEqual(node.Id, hbm2.NodeId);
				Assert.AreEqual(node.Id, hbm3.NodeId);

				Assert.AreEqual(1, hbm1.WorkBufferSize);
				Assert.AreEqual(0, hbm2.WorkBufferSize);
				Assert.AreEqual(0, hbm3.WorkBufferSize);

				Assert.AreEqual(task1, hbm1.CompletedTasks.Single());
				Assert.AreEqual(task2, hbm2.CompletedTasks.Single());
				Assert.Null(hbm3.CompletedTasks);
			}

			[Test]
			public void Test_HandleHeartBeatResponse()
			{
				var node = new Node(0.1, 0.1);
				node.HandleHeartBeatResponse(new HeartBeatResponse(node.Id));
				Assert.AreEqual(0, node.workBuffer.Size);
				node.HandleHeartBeatResponse(new HeartBeatResponse(node.Id, new List<Task>()));
				Assert.AreEqual(0, node.workBuffer.Size);
				node.HandleHeartBeatResponse(new HeartBeatResponse(node.Id, new List<Task>{new Task(34534)}));
				Assert.AreEqual(1, node.workBuffer.Size);
			}

			[Test]
			public void Test_GetCalculationTime()
			{
				var node = new Node(100, 0.1);
				Assert.AreEqual(TimeSpan.FromMilliseconds(50), node.GetCalculationTime(new Task(5000)));
				Assert.AreEqual(TimeSpan.FromMilliseconds(0.01), node.GetCalculationTime(new Task(1)));
			}

			[Test]
			public void Test_WorkBufferNotSerialized()
			{
				var node = new Node(0.1, 0.1);
				node.workBuffer.PutTask(Guid.NewGuid(), TimeSpan.Zero);
				Assert.NotNull(node);
				Assert.AreEqual(1, node.workBuffer.Size);

				var stream = new MemoryStream();
				var formatter = new BinaryFormatter();
				formatter.Serialize(stream, node);
				stream.Seek(0, SeekOrigin.Begin);
				var deserializedNode = (Node) formatter.Deserialize(stream);
				
				Assert.NotNull(deserializedNode.workBuffer);
				Assert.AreEqual(0, deserializedNode.workBuffer.Size);
				Assert.AreEqual(node.failurePattern, deserializedNode.failurePattern);
			}
		}
	}
}