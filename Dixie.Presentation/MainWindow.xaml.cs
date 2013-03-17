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
			presentationEngine = new DixiePresentationEngine(model);
		}

		#region Event handlers
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
			if (initialStateLabel.Text == InitialStateDescriptionConverter.Loaded)
				initialStateLabel.Background = okBrush;
			else initialStateLabel.Background = errorBrush;
		} 
		#endregion

		private void GenerateState(object sender, RoutedEventArgs e)
		{
			presentationEngine.GenerateNewState();
		}

		private readonly DixieModel model;
		private readonly DixiePresentationEngine presentationEngine;
		private static readonly SolidColorBrush okBrush = new SolidColorBrush(Colors.LightGreen);
		private static readonly SolidColorBrush errorBrush = new SolidColorBrush(Colors.LightCoral);
	}
}
