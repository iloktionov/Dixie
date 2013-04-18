using System.ComponentModel.Composition;

namespace Dixie.Core
{
	[Export(typeof(ISchedulerAlgorithm))]
	internal class UptimeMCTAlgorithm : WeightedMCTAlgorithmBase
	{
		public UptimeMCTAlgorithm()
			: base("UptimeMCTAlgorithm", new UptimeWeightSelector()) { }
	}
}