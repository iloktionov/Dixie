﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Dixie.Core
{
	internal class WorkBuffer
	{
		public WorkBuffer()
		{
			watch = Stopwatch.StartNew();
			records = new Dictionary<Guid, TimeSpan>();
		}

		public void PutTask(Guid taskId, TimeSpan calculationTime)
		{
			records.Add(taskId, watch.Elapsed + calculationTime);
		}

		public List<Guid> PopCompletedOrNull()
		{
			List<Guid> result = null;
			TimeSpan timeElapsed = watch.Elapsed;
			foreach (KeyValuePair<Guid, TimeSpan> pair in records)
				if (timeElapsed >= pair.Value)
				{
					if (result == null)
						result = new List<Guid>();
					result.Add(pair.Key);
				}
			if (result != null)
				result.ForEach(taskId => records.Remove(taskId));
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
		private readonly Dictionary<Guid, TimeSpan> records; 
	}
}