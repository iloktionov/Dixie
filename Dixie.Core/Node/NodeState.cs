using System;

namespace Dixie.Core
{
	public class NodeState
	{
		public NodeState(Guid id, double performance, double failureProbability, int workBufferSize)
		{
			Id = id;
			Performance = performance;
			FailureProbability = failureProbability;
			WorkBufferSize = workBufferSize;
		}

		public Guid Id { get; private set; }
		public Double Performance { get; private set; }
		public Double FailureProbability { get; private set; }
		public Int32 WorkBufferSize { get; private set; }
	}
}