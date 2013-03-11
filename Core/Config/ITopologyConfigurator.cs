using System;

namespace Dixie.Core
{
	internal interface ITopologyConfigurator
	{
		int GenerateChildrenCount();
		double GeneratePerformance();
		double GenerateFailureProbability();
		TimeSpan GenerateLinkLatency();
		TimeSpan GenerateOfflineTime(NodeFailureType failureType);
	}
}