using System;
using System.Collections.Generic;
using System.IO;
using Dixie.Core;

namespace Dixie.Console
{
	internal abstract class ConsoleUtilityBase
	{
		protected ConsoleUtilityBase(string helpFileName, ILog log)
		{
			this.helpFileName = helpFileName;
			this.log = log;
			actions = new Dictionary<string, NamedParameterAction>();
		}

		public void Work(ConsoleParameters parameters)
		{
			MapActionsToNamedParameters();
			if (parameters.NamedParamsCount <= 0)
			{
				RunWithoutNamedParameters();
				return;
			}
			foreach (KeyValuePair<string, NamedParameterAction> pair in actions)
				if (parameters.HasParam(pair.Key))
				{
					if (parameters.GetValuesCount(pair.Key) < pair.Value.MinArguments)
					{
						PrintHelp();
						return;
					}
					pair.Value.Action(pair.Key, parameters);
					return;
				}
			PrintHelp();
		}

		protected abstract void RunWithoutNamedParameters();

		protected abstract void MapActionsToNamedParameters();

		protected void MapActionToNamedParameter(string parameter, Action<string, ConsoleParameters> action, int minArguments)
		{
			actions[parameter] = new NamedParameterAction(action, minArguments);
		}

		protected void PrintHelp()
		{
			if (File.Exists(helpFileName))
				System.Console.Out.WriteLine(File.ReadAllText(helpFileName));
			else System.Console.Out.WriteLine("Where is help file {0}?", helpFileName);
		}

		protected readonly string helpFileName;
		protected readonly ILog log;
		private readonly Dictionary<string, NamedParameterAction> actions;

		#region NamedParameterAction
		private class NamedParameterAction
		{
			public NamedParameterAction(Action<string, ConsoleParameters> action, int minArguments)
			{
				Action = action;
				MinArguments = minArguments;
			}

			public Action<string, ConsoleParameters> Action { get; private set; }
			public int MinArguments { get; private set; }
		} 
		#endregion
	}
}