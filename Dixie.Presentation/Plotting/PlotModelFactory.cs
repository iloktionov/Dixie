using System;
using OxyPlot;

namespace Dixie.Presentation
{
	internal static class PlotModelFactory
	{
		public static PlotModel CreateEmpty()
		{
			var plotModel = new PlotModel("Total work done");
			plotModel.Axes.Add(new LinearAxis(AxisPosition.Left, Double.NaN, Double.NaN, "Work units"));
			plotModel.Axes.Add(new LinearAxis(AxisPosition.Bottom, Double.NaN, Double.NaN, "Time, sec"));
			return plotModel;
		}
	}
}