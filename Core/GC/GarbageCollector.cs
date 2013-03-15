using System;
using System.Collections.Generic;

namespace Dixie.Core
{
	// (iloktionov): Нужен для того, чтобы информация о навсегда удалённых из топологии нодах не копилась снежным комом во время долгих тестов.
	internal class GarbageCollector
	{
		public GarbageCollector()
		{
			permanentlyDeletedNodes = new List<Guid>();
			syncObject = new object();
		}

		public void AddStaleNode(Guid nodeId)
		{
			lock (syncObject)
				permanentlyDeletedNodes.Add(nodeId);
		}

		public void CollectGarbage(Master master, HeartBeatProcessor hbProcessor)
		{
			lock (syncObject)
			{
				// TODO(iloktionov): implement
				permanentlyDeletedNodes.Clear();
			}
		}

		private readonly List<Guid> permanentlyDeletedNodes;
		private readonly object syncObject;
	}
}