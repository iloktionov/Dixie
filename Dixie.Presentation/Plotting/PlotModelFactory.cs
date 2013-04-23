using System;
using OxyPlot;

namespace Dixie.Presentation
{
	internal static class PlotModelFactory
	{
		public static PlotModel CreateEmpty()
		{
			var plotModel = new PlotModel("Всего выполнено работы");
			plotModel.Axes.Add(new LinearAxis(AxisPosition.Left, Double.NaN, Double.NaN, "Единицы работы"));
			plotModel.Axes.Add(new LinearAxis(AxisPosition.Bottom, Double.NaN, Double.NaN, "Время, сек."));
			plotModel.LegendPlacement = LegendPlacement.Outside;
			return plotModel;
		}
	}
}