using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Dixie.Core
{
	internal partial class WorkBuffer
	{
		public WorkBuffer()
		{
			watch = Stopwatch.StartNew();
			records = new Queue<KeyValuePair<Guid, TimeSpan>>();
		}

		public void PutTask(Guid taskId, TimeSpan calculationTime)
		{
			TimeSpan previousTaskTS = records.Count > 0 ? records.Last().Value : watch.Elapsed;
			records.Enqueue(new KeyValuePair<Guid, TimeSpan>(taskId, previousTaskTS + calculationTime));
		}

		public List<Guid> PopCompletedOrNull()
		{
			List<Guid> result = null;
			TimeSpan timeElapsed = watch.Elapsed;
			while (records.Count > 0)
			{
				if (timeElapsed >= records.Peek().Value)
				{
					if (result == null)
						result = new List<Guid>();
					result.Add(records.Dequeue().Key);
				}
				else break;
			}
			return result;
		}

		public bool IsComputing()
		{
			return watch.IsRunning;
		}

		public void StopComputing()
		{
			if (watch.IsRunning)
				watch.Stop();
		}

		public void ResumeComputing()
		{
			if (!watch.IsRunning)
				watch.Start();
		}

		public int Size { get { return records.Count; } }

		private readonly Stopwatch watch;
		private readonly Queue<KeyValuePair<Guid, TimeSpan>> records;
	}
}