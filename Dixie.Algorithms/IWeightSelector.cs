using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	internal interface IWeightSelector
	{
		void Reset();
		void Update(List<NodeInfo> aliveNodes);
		/// <summary>
		/// Weight is multiplicative. Weight == 0.5 means that execution will be considered twice faster.
		/// </summary>
		Double GetWeight(Guid nodeId);
	}
}