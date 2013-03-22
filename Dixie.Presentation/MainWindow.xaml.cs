using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Dixie.Core;
using Microsoft.Win32;

namespace Dixie.Presentation
{
	public partial class MainWindow
	{
		public MainWindow()
		{
			model = new DixieModel();
			InitializeComponent();
			DataContext = model;
			topologyGraphLayout.SetupLayoutParameters();
			presentationEngine = new DixiePresentationEngine(model, testProgressBar, Dispatcher);
		}

		private void SelectAlgorithmButtonClick(object sender, RoutedEventArgs e)
		{
			foreach (object selectedAlgorithm in availableAlgorithmsBox.SelectedItems)
				selectedAlgorithmsBox.Items.Add(selectedAlgorithm);
			OnSelectedAlgorithmsBoxItemsChanged();
		}

		private void RemoveAlgorithmButtonClick(object sender, RoutedEventArgs e)
		{
			if (selectedAlgorithmsBox.SelectedIndex >= 0)
			{
				selectedAlgorithmsBox.Items.RemoveAt(selectedAlgorithmsBox.SelectedIndex);
				OnSelectedAlgorithmsBoxItemsChanged();
			}
		}

		private void OnSelectedAlgorithmsBoxItemsChanged()
		{
			selectedAlgorithmsBox.Background = selectedAlgorithmsBox.Items.Count > 0 ? okBrush : errorBrush;
		}

		private void DurationInputTextChanged(object sender, TextChangedEventArgs e)
		{
			OnTimeInputChangedInternal(durationInput);
		}

		private void CheckPeriodInputTextChanged(object sender, TextChangedEventArgs e)
		{
			OnTimeInputChangedInternal(checkPeriodInput);
		}

		private static void OnTimeInputChangedInternal(TextBox control)
		{
			try
			{
				TimeSpanParser.Parse(control.Text);
				control.Background = okBrush;
			}
			catch (FormatException)
			{
				control.Background = errorBrush;
			}
		}

		private void InitialStateTextChanged(object sender, TextChangedEventArgs e)
		{
			initialStateLabel.Background = initialStateLabel.Text == InitialStateDescriptionConverter.Loaded ? okBrush : errorBrush;
		}

		private void GenerateState(object sender, RoutedEventArgs e)
		{
			presentationEngine.GenerateNewState();
		}

		private void ReadStateFromFile(object sender, RoutedEventArgs e)
		{
			var dialog = new OpenFileDialog {InitialDirectory = AppDomain.CurrentDomain.BaseDirectory};
			bool? showResult = dialog.ShowDialog();
			if (!showResult.Value)
				return;
			using (Stream fileStream = dialog.OpenFile())
				presentationEngine.InitializeStateFromFile(fileStream);
		}

		private void SaveStateFromFile(object sender, RoutedEventArgs e)
		{
			if (!model.HasInitialState)
			{
				MessageBox.Show("Nothing to save.");
				return;
			}
			var dialog = new SaveFileDialog {InitialDirectory = AppDomain.CurrentDomain.BaseDirectory};
			bool? showResult = dialog.ShowDialog();
			if (!showResult.Value)
				return;
			using (Stream fileStream = dialog.OpenFile())
				presentationEngine.Engine.InitialState.Serialize(fileStream);
			MessageBox.Show("Saved successfully.");
		}

		private void StartTest(object sender, RoutedEventArgs e)
		{
			if (!model.HasInitialState)
			{
				MessageBox.Show("Initial state is not loaded yet.");
				return;
			}
			if (selectedAlgorithmsBox.Items.Count <= 0)
			{
				MessageBox.Show("Need to select at least one algorithm.");
				return;
			}
			TimeSpan testDuration;
			TimeSpan resultCheckPeriod;
			try
			{
				testDuration = TimeSpanParser.Parse(durationInput.Text);
				resultCheckPeriod = TimeSpanParser.Parse(checkPeriodInput.Text);
			}
			catch (FormatException error)
			{
				MessageBox.Show(error.ToString());
				return;
			}
			List<ISchedulerAlgorithm> algorithms = selectedAlgorithmsBox.Items.Cast<ISchedulerAlgorithm>().ToList();
			SetControlsState(false, startTestButton, resetButton, generateStateButton, loadStateButton, selectAlgorithmButton, removeAlgorithmButton);
			SetControlsState(true, stopTestButton);
			presentationEngine.Start(algorithms, testDuration, resultCheckPeriod, OnTestSuccess, OnTestError);
		}

		private void OnTestSuccess(ComparisonTestResult result)
		{
			MessageBox.Show(result.ToString());
			SetControlsState(false, stopTestButton);
			SetControlsState(true, startTestButton, resetButton, generateStateButton, loadStateButton, selectAlgorithmButton, removeAlgorithmButton);
		}

		private void OnTestError(Exception error)
		{
			MessageBox.Show(error.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
			SetControlsState(false, stopTestButton);
			SetControlsState(true, startTestButton, resetButton, generateStateButton, loadStateButton, selectAlgorithmButton, removeAlgorithmButton);
		}

		private void StopTest(object sender, RoutedEventArgs e)
		{
			presentationEngine.Stop();
			SetControlsState(false, stopTestButton);
			SetControlsState(true, startTestButton, resetButton, generateStateButton, loadStateButton, selectAlgorithmButton, removeAlgorithmButton);
		}

		private void ResetModel(object sender, RoutedEventArgs e)
		{
			presentationEngine.Reset();
			model.HasInitialState = false;
			selectedAlgorithmsBox.Items.Clear();
			OnSelectedAlgorithmsBoxItemsChanged();
		}

		private void SetControlsState(bool enabled, params Control[] controls)
		{
			foreach (Control control in controls)
				Dispatcher.Invoke(DispatcherPriority.Normal, new Action(() => control.IsEnabled = enabled));
		}

		private readonly DixieModel model;
		private readonly DixiePresentationEngine presentationEngine;
		private static readonly SolidColorBrush okBrush = new SolidColorBrush(Colors.LightGreen);
		private static readonly SolidColorBrush errorBrush = new SolidColorBrush(Colors.LightCoral);
	}
}
