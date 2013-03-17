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
			LoadAlgorithms();
			topologyObserver = new DixieTopologyObserver(dixieModel);
			tasksObserver = new DixieTasksObserver(dixieModel);
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
			tasksObserver.TryUpdateTaskStates(engine.Master);
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
				tasksObserver.TryUpdateTaskStates(engine.Master);
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
			tasksObserver.TryUpdateTaskStates(engine.Master);

			testProgressBar.Minimum = 0;
			testProgressBar.Maximum = testDuration.TotalMilliseconds * algorithms.Count;
			watch.Restart();

			engineThread = ThreadRunner.Run(() =>
				{
					ComparisonTestResult result = engine.TestAlgorithms(algorithms, testDuration, resultCheckperiod);
					ThreadPool.QueueUserWorkItem(obj => OnTestSuccess(result));
				}, null, OnErrorInEngine);
			graphUpdateThread = ThreadRunner.RunPeriodicAction(() => topologyObserver.TryUpdateModelGraph(engine.Topology), TopologyGraphUpdatePeriod);
			tasksUpdateThread = ThreadRunner.RunPeriodicAction(() => tasksObserver.TryUpdateTaskStates(engine.Master), TaskStatesUpdatePeriod);
		}
		
		public void Stop()
		{
			engine.Stop();
			ThreadRunner.StopThreads(engineThread, graphUpdateThread, tasksUpdateThread);
			engine.Stop();
			dispatcher.BeginInvoke(new Action(() => testProgressBar.Value = 0));
			foreach (ISchedulerAlgorithm algorithm in dixieModel.AvailableAlgorithms)
				AlgorithmNamesHelper.RestoreAlgorithmName(algorithm);
		}

		public void Reset()
		{
			topologyObserver.Reset();
			tasksObserver.Reset();
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
			tasksObserver.TryUpdateTaskStates(engine.Master);
			onTestSuccess(result);
		}

		private void OnErrorInEngine(Exception error)
		{
			Stop();
			onTestError(error);
		}

		private void LoadAlgorithms()
		{
			try
			{
				dixieModel.AvailableAlgorithms = AlgorithmsContainer.GetAvailableAlgorithms();
			}
			catch (Exception error)
			{
				MessageBox.Show(String.Format("Error in loading algorithms: {0}", error));
				Application.Current.Shutdown(-1);
			}
		}

		private readonly DixieModel dixieModel;
		private readonly DixieTopologyObserver topologyObserver;
		private readonly DixieTasksObserver tasksObserver;
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
		private Thread tasksUpdateThread;

		private static readonly TimeSpan TopologyGraphUpdatePeriod = TimeSpan.FromMilliseconds(350);
		private static readonly TimeSpan TaskStatesUpdatePeriod = TimeSpan.FromMilliseconds(150);
	}
}