using System;
using System.Collections.Generic;
using System.Linq;

namespace Dixie.Core
{
	// (iloktionov): Нужен для того, чтобы информация о навсегда удалённых из топологии нодах не копилась снежным комом во время долгих тестов.
	internal class GarbageCollector
	{
		public GarbageCollector(TimeSpan deadabilityThreshold)
		{
			permanentlyDeletedNodes = new List<KeyValuePair<Guid, DateTime>>();
			syncObject = new object();
			safeTimeWindow = TimeSpan.FromTicks(deadabilityThreshold.Ticks * 4);
		}

		public void AddStaleNode(Guid nodeId)
		{
			lock (syncObject)
				permanentlyDeletedNodes.Add(new KeyValuePair<Guid, DateTime>(nodeId, DateTime.Now));
		}

		public void CollectGarbage(Master master)
		{
			lock (syncObject)
			{
				DateTime now = DateTime.Now;
				var readyForCollection = new HashSet<Guid>(permanentlyDeletedNodes
					.Where(pair => now - pair.Value >= safeTimeWindow)
					.Select(pair => pair.Key));
				master.CollectGarbage(readyForCollection);
				permanentlyDeletedNodes.RemoveAll(pair => readyForCollection.Contains(pair.Key));
			}
		}

		public int Count { get { return permanentlyDeletedNodes.Count; } }

		private readonly List<KeyValuePair<Guid, DateTime>> permanentlyDeletedNodes;
		private readonly TimeSpan safeTimeWindow;
		private readonly object syncObject;
	}
}