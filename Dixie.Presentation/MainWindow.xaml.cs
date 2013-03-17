using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Dixie.Core;

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
			model.AvailableAlgorithms = AlgorithmsContainer.GetAvailableAlgorithms();
			presentationEngine = new DixiePresentationEngine(model);
			presentationEngine.Start();
		}

		private readonly DixieModel model;
		private readonly DixiePresentationEngine presentationEngine;

		private void SelectAlgorithmButtonClick(object sender, RoutedEventArgs e)
		{
			foreach (object selectedAlgorithm in availableAlgorithmsBox.SelectedItems)
				selectedAlgorithmsBox.Items.Add(selectedAlgorithm);
		}

		private void RemoveAlgorithmButtonClick(object sender, RoutedEventArgs e)
		{
			if (selectedAlgorithmsBox.SelectedIndex >= 0)
				selectedAlgorithmsBox.Items.RemoveAt(selectedAlgorithmsBox.SelectedIndex);
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
				control.Background = new SolidColorBrush(Colors.LightGreen);
			}
			catch (FormatException)
			{
				control.Background = new SolidColorBrush(Colors.LightCoral);
			}
		}
	}
}
