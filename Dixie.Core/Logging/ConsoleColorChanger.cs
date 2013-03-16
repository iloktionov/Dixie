using System;

namespace Dixie.Core
{
	internal class ConsoleColorChanger : IDisposable
	{
		public ConsoleColorChanger(ConsoleColor newColor)
		{
			originalColor = Console.ForegroundColor;
			Console.ForegroundColor = newColor;
		}

		public void Dispose()
		{
			Console.ForegroundColor = originalColor;
		}

		private readonly ConsoleColor originalColor;
	}
}