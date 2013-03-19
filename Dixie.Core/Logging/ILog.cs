namespace Dixie.Core
{
	internal interface ILog
	{
		void Debug(string format, params object[] args);
		void Info(string format, params object[] args);
		void Warn(string format, params object[] args);
		void Error(string format, params object[] args);

		bool IsDebugEnabled { get; set; }
	}
}
