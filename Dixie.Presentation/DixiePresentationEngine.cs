using System;
using System.IO;
using System.Windows;
using Dixie.Core;

namespace Dixie.Presentation
{
	internal class DixiePresentationEngine
	{
		public DixiePresentationEngine(DixieModel dixieModel)
		{
			this.dixieModel = dixieModel;
			dixieModel.AvailableAlgorithms = AlgorithmsContainer.GetAvailableAlgorithms();
			topologyObserver = new DixieTopologyObserver(dixieModel);
			plotManager = new PlotManager(dixieModel);
			random = new Random();
			log = new FileBasedLog("Dixie.log");
		}

		public void GenerateNewState()
		{
			Reset();
			InitialGridState initialState = InitialGridState.GenerateNew(random.Next(15, 35), random);
			engine = new Engine(initialState, log);
			topologyObserver.TryUpdateModelGraph(initialState.Topology);
			dixieModel.HasInitialState = true;
		}

		public void InitializeStateFromFile(Stream fileStream)
		{
			try
			{
				Reset();
				InitialGridState initialState = InitialGridState.Deserialize(fileStream);
				engine = new Engine(initialState, log);
				topologyObserver.TryUpdateModelGraph(initialState.Topology);
				dixieModel.HasInitialState = true;
			}
			catch (Exception error)
			{
				MessageBox.Show(String.Format("An error has occured in reading initial state from file: {0}", error));
				dixieModel.HasInitialState = false;
			}
		}

		public void Start()
		{
//			ISchedulerAlgorithm algorithm = new RandomAlgorithm();
//			gridEngine = new Engine(InitialGridState.GenerateNew(20), new FileBasedLog("test.log"));
//			gridEngine.SetOnIntermediateResultCallback(OnIntermediateTestResult);
//			gridEngineThread = ThreadRunner.Run(() => gridEngine.TestAlgorithms(new List<ISchedulerAlgorithm>{algorithm, algorithm, algorithm, algorithm, algorithm},
//				TimeSpan.FromSeconds(5), TimeSpan.FromMilliseconds(100)));
//			modelUpdateThread = ThreadRunner.RunPeriodicAction(() => topologyObserver.TryUpdateModelGraph(gridEngine), ModelUpdatePeriod);
		}

		public void Stop()
		{
			
		}

		public void Reset()
		{
			topologyObserver.Reset();
			plotManager.Reset();
			engine = null;
		}

		private void OnIntermediateTestResult(IntermediateTestResult result, string algorithmName)
		{
			plotManager.AddPointToSeries(algorithmName, result.TimeElapsed.TotalSeconds, result.WorkDone);
		}

		private readonly DixieModel dixieModel;
		private readonly DixieTopologyObserver topologyObserver;
		private readonly PlotManager plotManager;
		private readonly Random random;
		private readonly ILog log;
		private Engine engine;

		private static readonly TimeSpan ModelUpdatePeriod = TimeSpan.FromMilliseconds(500);
	}
}