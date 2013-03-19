namespace Dixie.Core
{
	internal class PrefixedILogWrapper : ILog
	{
		public PrefixedILogWrapper(ILog baseLog, string prefix)
		{
			this.baseLog = baseLog;
			this.prefix = "[" + prefix + "] ";
		}

		public void Debug(string format, params object[] args)
		{
			if (baseLog.IsDebugEnabled)
				baseLog.Debug(prefix + format, args);
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

		public bool IsDebugEnabled
		{
			get { return baseLog.IsDebugEnabled; }
			set { baseLog.IsDebugEnabled = value; }
		}

		private readonly ILog baseLog;
		private readonly string prefix;
	}
}