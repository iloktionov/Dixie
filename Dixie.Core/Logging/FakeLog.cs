namespace Dixie.Core
{
	internal class FakeLog : ILog
	{
		public void Debug(string format, params object[] args) { }

		public void Info(string format, params object[] args) { }

		public void Warn(string format, params object[] args) { }

		public void Error(string format, params object[] args) { }

		public bool IsDebugEnabled { get; set; }
	}
}