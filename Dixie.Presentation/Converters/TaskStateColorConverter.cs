using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Dixie.Core;

namespace Dixie.Presentation
{
	internal class TaskStateColorConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var status = (TaskStatus) value;
			switch (status)
			{
				case TaskStatus.Pending:
					return pendingBrush;
				case TaskStatus.Assigned:
					return assignedBrush;
				case TaskStatus.Completed:
					return completedBrush;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private static readonly SolidColorBrush pendingBrush = new SolidColorBrush(Colors.Gray);
		private static readonly SolidColorBrush assignedBrush = new SolidColorBrush(Colors.Orange);
		private static readonly SolidColorBrush completedBrush = new SolidColorBrush(Colors.LightGreen);
	}
}