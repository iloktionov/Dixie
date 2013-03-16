using System;
using System.Threading;
using Dixie.Core;

namespace Dixie.Presentation
{
	internal class DixiePresentationEngine
	{
		public DixiePresentationEngine(DixieModel model)
		{
			graphObserver = new DixieGraphObserver(model);
		}

		public void InitializeFromFile(string filename)
		{
			
		}

		public void Start()
		{
			gridEngine = new Engine(InitialGridState.GenerateNew(20), new FakeLog());
			gridEngineThread = ThreadRunner.Run(() => gridEngine.TestAlgorithm(new RandomAlgorithm(), TimeSpan.FromHours(1), TimeSpan.FromSeconds(1)));
			modelUpdateThread = ThreadRunner.RunPeriodicAction(() => graphObserver.TryUpdateModelGraph(gridEngine), ModelUpdatePeriod);
		}

		public void Stop()
		{
			if (gridEngineThread != null)
			{
				gridEngineThread.Abort();
				gridEngineThread.Join();
			}
			if (modelUpdateThread != null)
			{
				modelUpdateThread.Abort();
				modelUpdateThread.Join();
			}
		}

		public void Reset()
		{
			
		}

		private readonly DixieGraphObserver graphObserver;
		private Engine gridEngine;
		private Thread gridEngineThread;
		private Thread modelUpdateThread;

		private static readonly TimeSpan ModelUpdatePeriod = TimeSpan.FromMilliseconds(500);
	}
}