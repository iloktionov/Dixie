using System;

namespace Dixie.Core
{
	internal interface ITopologyConfigurator
	{
		double GeneratePerformance();
		double GenerateFailureProbability();
		TimeSpan GenerateLinkLatency();
		TimeSpan GenerateOfflineTime(NodeFailureType failureType);
	}
}