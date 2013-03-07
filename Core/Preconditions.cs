using System;

namespace Dixie.Core
{
	public static class Preconditions
	{
		public static void CheckNotNull<T>(T argument) where T : class
		{
			if (argument == null)
				throw new ArgumentNullException();
		}

		public static void CheckNotNull<T>(T argument, string argumentName) where T : class
		{
			if (argument == null)
				throw new ArgumentNullException(argumentName);
		}

		public static void CheckArgument(bool argumentCondition)
		{
			if (!argumentCondition)
				throw new ArgumentException();
		}

		public static void CheckArgument(bool argumentCondition, string argumentName, string errorMessageFormat, params object[] errorMessageArgs)
		{
			if (!argumentCondition)
				throw new ArgumentException(String.Format(errorMessageFormat, errorMessageArgs), argumentName);
		} 
	}
}