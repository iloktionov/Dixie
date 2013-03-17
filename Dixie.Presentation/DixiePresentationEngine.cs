using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Dixie.Core;

namespace Dixie.Presentation
{
	internal class DixiePresentationEngine
	{
		public DixiePresentationEngine(DixieModel dixieModel, ProgressBar testProgressBar, Dispatcher dispatcher)
		{
			this.dixieModel = dixieModel;
			this.testProgressBar = testProgressBar;
			this.dispatcher = dispatcher;
			dixieModel.AvailableAlgorithms = AlgorithmsContainer.GetAvailableAlgorithms();
			topologyObserver = new DixieTopologyObserver(dixieModel);
			plotManager = new PlotManager(dixieModel);
			random = new Random();
			log = new FileBasedLog("Dixie.log");
			watch = Stopwatch.StartNew();
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

			testProgressBar.Minimum = 0;
			testProgressBar.Maximum = testDuration.TotalMilliseconds * algorithms.Count;
			watch.Restart();

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
			dispatcher.BeginInvoke(new Action(() => testProgressBar.Value = 0));
		}

		public void Reset()
		{
			topologyObserver.Reset();
			plotManager.Reset();
			dispatcher.BeginInvoke(new Action(() => testProgressBar.Value = 0));
		}

		private void OnIntermediateTestResult(IntermediateTestResult result, string algorithmName)
		{
			plotManager.AddPointToSeries(algorithmName, result.TimeElapsed.TotalSeconds, result.WorkDone);
			dispatcher.BeginInvoke(new Action(() => testProgressBar.Value = watch.Elapsed.TotalMilliseconds));
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
		private readonly ProgressBar testProgressBar;
		private readonly Dispatcher dispatcher;
		private readonly Stopwatch watch;

		private Engine engine;
		private Action<ComparisonTestResult> onTestSuccess;
		private Action<Exception> onTestError;
		private Thread engineThread;
		private Thread graphUpdateThread;

		private static readonly TimeSpan TopologyGraphUpdatePeriod = TimeSpan.FromMilliseconds(350);
	}
}