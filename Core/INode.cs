using System;

namespace Dixie.Core
{
	public interface INode
	{
		Guid Id { get; }
		Double Performance { get; }
		Double FailureProbability { get; }
	}
}
