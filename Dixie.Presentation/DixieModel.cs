﻿using System.Collections.Generic;
using System.ComponentModel;
using Dixie.Core;
using OxyPlot;

namespace Dixie.Presentation
{
	internal class DixieModel : INotifyPropertyChanged
	{
		public DixieGraph TopologyGraph
		{
			get { return topologyGraph; }
			set
			{
				topologyGraph = value;
				RaisePropertyChanged("TopologyGraph");
			}
		}

		public PlotModel PlotModel
		{
			get { return plotModel; }
			set
			{
				plotModel = value;
				RaisePropertyChanged("PlotModel");
			}
		}

		public bool HasInitialState
		{
			get { return hasInitialState; }
			set
			{
				hasInitialState = value;
				RaisePropertyChanged("HasInitialState");
			}
		}

		public List<ISchedulerAlgorithm> AvailableAlgorithms
		{
			get { return availableAlgorithms; }
			set
			{
				availableAlgorithms = value;
				RaisePropertyChanged("AvailableAlgorithms");
			}
		}

		public IEnumerable<TaskState> TaskStates
		{
			get { return taskStates; }
			set
			{
				taskStates = value;
				RaisePropertyChanged("TaskStates");
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChanged(string property)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(property));
		}

		private DixieGraph topologyGraph;
		private PlotModel plotModel;
		private IEnumerable<TaskState> taskStates;
		private bool hasInitialState;
		private List<ISchedulerAlgorithm> availableAlgorithms;
	}
}