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
			if (state.Status != TaskStatus.Assigned)
				return state.Status.ToString();
			return String.Format("Assigned {0}{1} node(s)", Environment.NewLine, state.AssignedNodes.Count);
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}