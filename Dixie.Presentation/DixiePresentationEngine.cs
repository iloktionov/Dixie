using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
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

		public void Start(List<ISchedulerAlgorithm> algorithms, TimeSpan testDuration, TimeSpan resultCheckperiod, Action<ComparisonTestResult> onSuccess, Action<Exception> onError)
		{
			onTestSuccess = onSuccess;
			onTestError = onError;
			Reset();
			engine.SetOnIntermediateResultCallback(OnIntermediateTestResult);
			topologyObserver.TryUpdateModelGraph(engine.InitialState.Topology);
			engineThread = ThreadRunner.Run(() =>
				{
					ComparisonTestResult result = engine.TestAlgorithms(algorithms, testDuration, resultCheckperiod);
					ThreadPool.QueueUserWorkItem(obj => OnTestSuccess(result));
				}, null, OnErrorInEngine);
			graphUpdateThread = ThreadRunner.RunPeriodicAction(() => topologyObserver.TryUpdateModelGraph(engine.Topology), TopologyGraphUpdatePeriod);
		}
		
		public void Stop()
		{
			engine.Stop();
			ThreadRunner.StopThreads(engineThread, graphUpdateThread);
			engine.Stop();
		}

		public void Reset()
		{
			topologyObserver.Reset();
			plotManager.Reset();
		}

		private void OnIntermediateTestResult(IntermediateTestResult result, string algorithmName)
		{
			plotManager.AddPointToSeries(algorithmName, result.TimeElapsed.TotalSeconds, result.WorkDone);
		}

		private void OnTestSuccess(ComparisonTestResult result)
		{
			Stop();
			onTestSuccess(result);
		}

		private void OnErrorInEngine(Exception error)
		{
			Stop();
			onTestError(error);
		}

		private readonly DixieModel dixieModel;
		private readonly DixieTopologyObserver topologyObserver;
		private readonly PlotManager plotManager;
		private readonly Random random;
		private readonly ILog log;

		private Engine engine;
		private Action<ComparisonTestResult> onTestSuccess;
		private Action<Exception> onTestError;
		private Thread engineThread;
		private Thread graphUpdateThread;

		private static readonly TimeSpan TopologyGraphUpdatePeriod = TimeSpan.FromMilliseconds(350);
	}
}