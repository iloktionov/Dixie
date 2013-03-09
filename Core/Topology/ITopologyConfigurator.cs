using System;

namespace Dixie.Core
{
	public interface ITopologyConfigurator
	{
		int GenerateChildrenCount();
		double GeneratePerformance();
		double GenerateFailureProbability();
		TimeSpan GenerateLinkLatency();
	}
}