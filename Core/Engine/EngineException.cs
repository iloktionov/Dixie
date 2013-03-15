using System;

namespace Dixie.Core
{
	public class EngineException : Exception
	{
		public EngineException(string message) 
			: base(message)
		{
		}
	}
}