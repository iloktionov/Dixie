using System;

namespace Dixie.Core
{
	internal interface INode
	{
		Guid Id { get; }
		Double Performance { get; }
		Double FailureProbability { get; }
	}
}
