namespace Dixie.Core
{
	public class PrefixedILogWrapper : ILog
	{
		public PrefixedILogWrapper(ILog baseLog, string prefix)
		{
			this.baseLog = baseLog;
			this.prefix = "[" + prefix + "] ";
		}

		public void Info(string format, params object[] args)
		{
			baseLog.Info(prefix + format, args);
		}

		public void Warn(string format, params object[] args)
		{
			baseLog.Warn(prefix + format, args);
		}

		public void Error(string format, params object[] args)
		{
			baseLog.Error(prefix + format, args);
		}

		private readonly ILog baseLog;
		private readonly string prefix;
	}
}