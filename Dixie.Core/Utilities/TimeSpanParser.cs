using System;
using System.Globalization;

namespace Dixie.Core
{
	public static class TimeSpanParser
	{
		public static TimeSpan Parse(string input)
		{
			TimeSpan timespan;
			if (TimeSpan.TryParse(input, out timespan))
				return timespan;
			if (input.EndsWith(MilliSeconds, StringComparison.InvariantCultureIgnoreCase))
				return TimeSpan.FromMilliseconds(ParseDouble(input, MilliSeconds.Length));
			if (input.EndsWith(Seconds, StringComparison.InvariantCultureIgnoreCase))
				return TimeSpan.FromSeconds(ParseDouble(input, Seconds.Length));
			if (input.EndsWith(Minutes, StringComparison.InvariantCultureIgnoreCase))
				return TimeSpan.FromMinutes(ParseDouble(input, Minutes.Length));
			if (input.EndsWith(Hours, StringComparison.InvariantCultureIgnoreCase))
				return TimeSpan.FromHours(ParseDouble(input, Hours.Length));
			throw new FormatException(String.Format("TimeSpanParser. Failed to parse TimeSpan from string '{0}'.", input));
		}

		private static Double ParseDouble(string input, int trimLength)
		{
			string item = input.Substring(0, input.Length - trimLength);
			double res;
			if (Double.TryParse(item, NumberStyles.Float, new NumberFormatInfo { NumberDecimalSeparator = "," }, out res))
				return res;
			if (Double.TryParse(item, NumberStyles.Float, new NumberFormatInfo { NumberDecimalSeparator = "." }, out res))
				return res;
			throw new FormatException(String.Format("TimeSpanParser. Error in parsing string {0} to Double.", item));
		}

		private const string MilliSeconds = "ms";
		private const string Seconds = "s";
		private const string Minutes = "m";
		private const string Hours = "h";
	}
}