using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;

namespace Dixie.Core
{
	public partial class Node
	{
		[TestFixture]
		internal class Node_Tests
		{
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
			}
		}
	}
}