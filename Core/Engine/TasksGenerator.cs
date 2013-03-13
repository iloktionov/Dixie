using System;
using System.Collections.Generic;
using System.Linq;

namespace Dixie.Core
{
	internal class TasksGenerator
	{
		public TasksGenerator(InitialGridState initialState)
		{
			this.initialState = initialState;
			random = new Random(initialState.RandomSeed);
		}

		public IEnumerable<Task> GenerateTasks()
		{
			return Enumerable.Range(1, random.Next(initialState.Topology.WorkerNodesCount, initialState.Topology.WorkerNodesCount * 2))
				.Select(i => new Task(random.NextDouble(initialState.EngineSettings.MinTaskVolume, initialState.EngineSettings.MaxTaskVolume)))
				.ToList();
		}

		private readonly InitialGridState initialState;
		private readonly Random random;
	}
}