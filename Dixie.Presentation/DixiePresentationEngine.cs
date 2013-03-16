using System;
using System.Collections.Generic;
using System.Threading;
using Dixie.Core;

namespace Dixie.Presentation
{
	internal class DixiePresentationEngine
	{
		public DixiePresentationEngine(DixieModel model)
		{
			graphObserver = new DixieGraphObserver(model);
			plotManager = new PlotManager(model);
		}

		public void InitializeFromFile(string filename)
		{
			
		}

		public void Start()
		{
			ISchedulerAlgorithm algorithm = new RandomAlgorithm();
			gridEngine = new Engine(InitialGridState.GenerateNew(20), new FileBasedLog("test.log"));
			gridEngine.SetOnIntermediateResultCallback(OnIntermediateTestResult);
			gridEngineThread = ThreadRunner.Run(() => gridEngine.TestAlgorithms(new List<ISchedulerAlgorithm>{algorithm, algorithm, algorithm, algorithm, algorithm},
				TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(100)));
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

		private void OnIntermediateTestResult(IntermediateTestResult result, string algorithmName)
		{
			plotManager.AddPointToSeries(algorithmName, result.TimeElapsed.TotalSeconds, result.WorkDone);
		}

		private readonly DixieGraphObserver graphObserver;
		private PlotManager plotManager;
		private Engine gridEngine;
		private Thread gridEngineThread;
		private Thread modelUpdateThread;

		private static readonly TimeSpan ModelUpdatePeriod = TimeSpan.FromMilliseconds(500);
	}
}