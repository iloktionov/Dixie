using System;
using System.IO;
using System.Text;

namespace Dixie.Core
{
	public class FileBasedLog : ILog
	{
		public FileBasedLog(string fileName, FileMode fileMode = FileMode.Create, bool isDebugEnabled = true)
		{
			IsDebugEnabled = isDebugEnabled;
			var stream = new FileStream(fileName, fileMode, FileAccess.Write, FileShare.ReadWrite);
			writer = new StreamWriter(stream, Encoding.UTF8);
		}

		public void Debug(string format, params object[] args)
		{
			if (!IsDebugEnabled) 
				return;
			string message = FormatMessage("DEBUG " + format, args);
			lock (writer)
				writer.WriteLine(message);
		}

		public void Info(string format, params object[] args)
		{
			string message = FormatMessage("INFO " + format, args);
			lock (writer)
				writer.WriteLine(message);
		}

		public void Warn(string format, params object[] args)
		{
			string message = FormatMessage("WARN " + format, args);
			lock (writer)
				writer.WriteLine(message);
		}

		public void Error(string format, params object[] args)
		{
			string message = FormatMessage("ERROR " + format, args);
			lock (writer)
				writer.WriteLine(message);
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

		private readonly StreamWriter writer;
	}
}