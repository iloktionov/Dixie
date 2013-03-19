using System;

namespace Dixie.Core
{
	internal class EngineException : Exception
	{
		public EngineException(string message) 
			: base(message)
		{
		}
	}
}