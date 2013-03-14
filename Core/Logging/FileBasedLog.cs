using System;
using System.IO;
using System.Text;

namespace Dixie.Core
{
	public class FileBasedLog : ILog
	{
		public FileBasedLog(string fileName, FileMode fileMode = FileMode.Create)
		{
			var stream = new FileStream(fileName, fileMode, FileAccess.Write, FileShare.ReadWrite);
			writer = new StreamWriter(stream, Encoding.UTF8);
		}

		public void Info(string format, params object[] args)
		{
			writer.WriteLine(FormatMessage("INFO " + format, args));
		}

		public void Warn(string format, params object[] args)
		{
			writer.WriteLine(FormatMessage("WARN " + format, args));
		}

		public void Error(string format, params object[] args)
		{
			writer.WriteLine(FormatMessage("ERROR " + format, args));
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

		private readonly StreamWriter writer;
	}
}