using System;
using System.Globalization;
using System.Windows.Data;

namespace Dixie.Presentation
{
	internal class InitialStateDescriptionConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var isLoaded = (bool) value;
			return isLoaded ? Loaded : NotLoaded;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		public const string Loaded = "Loaded";
		public const string NotLoaded = "Not loaded";
	}
}