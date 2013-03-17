using System;
using System.Collections.Generic;
using OxyPlot;

namespace Dixie.Presentation
{
	internal class PlotManager
	{
		public PlotManager(DixieModel dixieModel)
		{
			this.dixieModel = dixieModel;
			Reset();
		}

		public void Reset()
		{
			dixieModel.PlotModel = PlotModelFactory.CreateEmpty();
			seriesDictionary = new Dictionary<string, LineSeries>();
		}

		public void AddPointToSeries(string seriesName, Double x, Double y)
		{
			LineSeries series;
			if (seriesDictionary.TryGetValue(seriesName, out series))
			{
				series.Points.Add(new DataPoint(x, y));
				if (dixieModel.PlotModel.PlotControl != null)
					dixieModel.PlotModel.RefreshPlot(true);
			}
			else
			{
				PlotModel newModel = PlotModelFactory.CreateEmpty();
				foreach (Series oldSeries in dixieModel.PlotModel.Series)
					newModel.Series.Add(oldSeries);
				series = new LineSeries(seriesName);
				series.Points.Add(new DataPoint(x, y));
				seriesDictionary.Add(seriesName, series);
				newModel.Series.Add(series);
				dixieModel.PlotModel = newModel;
			}
		}

		private readonly DixieModel dixieModel;
		private Dictionary<string, LineSeries> seriesDictionary;
	}
}