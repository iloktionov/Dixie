using System;
using System.Collections.Generic;
using System.Linq;

namespace Dixie.Core
{
	internal class TasksGenerator
	{
		public TasksGenerator(InitialGridState initialState, ILog log)
		{
			this.initialState = initialState;
			this.log = new PrefixedILogWrapper(log, "TasksGenerator");
			random = new Random(initialState.RandomSeed);
		}

		public List<Task> GenerateTasks()
		{
			int initialNodes = initialState.Topology.WorkerNodesCount;
			List<Task> result = 
				Enumerable.Range(1, random.Next(initialNodes, initialNodes * 2))
				.Select(i => GenerateTask())
				.ToList();
			LogResult(result);
			return result;
		}

		private Task GenerateTask()
		{
			return new Task(random.NextDouble(initialState.EngineSettings.MinTaskVolume, initialState.EngineSettings.MaxTaskVolume));
		}

		private void LogResult(List<Task> result)
		{
			log.Info("Generated {0} tasks of summary volume {1:0.000}", result.Count, result.Sum(task => task.Volume));
		}

		private readonly InitialGridState initialState;
		private readonly Random random;
		private readonly ILog log;
	}
}