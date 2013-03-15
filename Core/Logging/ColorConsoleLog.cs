using System;

namespace Dixie.Core
{
	public class ColorConsoleLog : ILog
	{
		public ColorConsoleLog(bool isDebugEnabled = true)
		{
			IsDebugEnabled = isDebugEnabled;
		}

		public void Debug(string format, params object[] args)
		{
			using (new ConsoleColorChanger(DebugColor))
				Console.Out.WriteLine(FormatMessage("DEBUG " + format, args));
		}

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

		public bool IsDebugEnabled { get; set; }

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

		private const ConsoleColor DebugColor = ConsoleColor.DarkGreen;
		private const ConsoleColor InfoColor = ConsoleColor.White;
		private const ConsoleColor WarnColor = ConsoleColor.Yellow;
		private const ConsoleColor ErrorColor = ConsoleColor.Red;
	}
}