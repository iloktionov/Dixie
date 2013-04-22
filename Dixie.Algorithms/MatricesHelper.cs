using System;
using System.Collections.Generic;
using System.Linq;

namespace Dixie.Core
{
	internal static class MatricesHelper
	{
		// (iloktionov): Элемент в позиции (i, j) соответствует времени выполнения i-го задания j-й машиной.
		public static Double[,] ConstructETCMatrix(List<NodeInfo> aliveNodes, List<Task> pendingTasks)
		{
			var etcMatrix = new Double[pendingTasks.Count, aliveNodes.Count];
			for (int i = 0; i < pendingTasks.Count; i++)
				for (int j = 0; j < aliveNodes.Count; j++)
					etcMatrix[i, j] = pendingTasks[i].Volume / aliveNodes[j].Performance;
			return etcMatrix;
		}

		// (iloktionov): Элемент в позиции i соответствует времени, оставшемуся до полной готовности i-й машины.
		public static Double[] ConstructAvailabilityVector(IEnumerable<NodeInfo> aliveNodes)
		{
			return aliveNodes.Select(info => info.AvailabilityTime.TotalMilliseconds).ToArray();
		}
	}
}