using System;

namespace Dixie.Core
{
	public class ColorConsoleLog : ILog
	{
		public void Info(string format, params object[] args)
		{
			using (new ConsoleColorChanger(InfoColor))
				Console.Out.WriteLine(FormatMessage("INFO " + format, args));
		}

		public void Warn(string format, params object[] args)
		{
			using (new ConsoleColorChanger(WarnColor))
				Console.Out.WriteLine(FormatMessage("WARN " + format, args));
		}

		public void Error(string format, params object[] args)
		{
			using (new ConsoleColorChanger(ErrorColor))
				Console.Out.WriteLine(FormatMessage("ERROR " + format, args));
		}

		private static string FormatMessage(string format, params object[] args)
		{
			try
			{
				return String.Format(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss,fff ") + format, args);
			}
			catch (Exception error)
			{
				return String.Format("Error in rendering format '{0}'. Exception: {1}", format, error);
			}
		}

		private const ConsoleColor InfoColor = ConsoleColor.White;
		private const ConsoleColor WarnColor = ConsoleColor.Yellow;
		private const ConsoleColor ErrorColor = ConsoleColor.Red;
	}
}