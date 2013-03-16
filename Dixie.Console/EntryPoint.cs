using Dixie.Core;

namespace Dixie.Console
{
	internal class EntryPoint
	{
		public static void Main(string[] args)
		{
			var parameters = new ConsoleParameters(args);
			var log = new ColorConsoleLog();
			var dixieUtility = new DixieConsoleUtility(log);
			dixieUtility.Work(parameters);
		}
	}
}
