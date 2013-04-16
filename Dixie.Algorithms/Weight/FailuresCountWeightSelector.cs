using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	internal class FailuresCountWeightSelector : IWeightSelector
	{
		public FailuresCountWeightSelector()
		{
			weights = new Dictionary<Guid, double>();
		}

		public void Reset() { }

		public double GetWeight(Guid nodeId)
		{
			return weights[nodeId];
		}

		public void Update(List<NodeInfo> aliveNodes)
		{
			weights.Clear();
			foreach (NodeInfo aliveNode in aliveNodes)
				weights.Add(aliveNode.Id, 1 + aliveNode.FailureHistory.Failures.Count);
		}

		private readonly Dictionary<Guid, Double> weights;
	}
}