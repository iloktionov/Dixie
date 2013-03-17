using System;
using System.Globalization;
using System.Windows.Data;
using Dixie.Core;

namespace Dixie.Presentation
{
	internal class TasksStateDescriptionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var state = (TaskState)value;
			string description = String.Format("{0:0.0}{1}{1}{2}", state.Task.Volume, Environment.NewLine, state.Status);
			if (state.Status == TaskStatus.Assigned)
				description += String.Format("{0}{1} node(s)", Environment.NewLine, state.AssignedNodes.Count);
			return description;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}