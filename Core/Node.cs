using System;

namespace Dixie.Core
{
	public class Node : INode
	{
		public Node(double performance, double failureProbability)
		{
			Performance = performance;
			FailureProbability = failureProbability;
			Id = Guid.NewGuid();
		}

		public Guid Id { get; private set; }
		public Double Performance { get; private set; }
		public Double FailureProbability { get; private set; }
	}
}