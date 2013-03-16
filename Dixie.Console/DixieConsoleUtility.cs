﻿using System;
using System.Collections.Generic;
using System.Linq;
using Dixie.Core;

namespace Dixie.Console
{
	internal class DixieConsoleUtility : ConsoleUtilityBase
	{
		public DixieConsoleUtility(ILog log)
			: base(HelpFileName, log) { }

		public const string HelpFileName = "Dixie.Console.help";

		protected override void RunWithoutNamedParameters()
		{
			PrintHelp();
		}

		protected override void MapActionsToNamedParameters()
		{
			MapActionToNamedParameter("--generateState", GenerateInitialState, 2);
			MapActionToNamedParameter("--updateStateSettings", UpdateStateSettings, 1);
			MapActionToNamedParameter("--listAlgorithms", ListAlgorithms, 0);
		}

		private static void GenerateInitialState(string consoleParameter, ConsoleParameters parameters)
		{
			int nodesCount = Int32.Parse(parameters.GetValue(consoleParameter, 0));
			string outputFilename = parameters.GetValue(consoleParameter, 1);
			InitialGridState.GenerateNew(nodesCount).SaveToFile(outputFilename);
		}

		private static void UpdateStateSettings(string consoleParameter, ConsoleParameters parameters)
		{
			string fileName = parameters.GetValue(consoleParameter, 0);
			InitialGridState deserializedState = InitialGridState.ReadFromFile(fileName);
			var newState = new InitialGridState(
				deserializedState.Topology, 
				deserializedState.RandomSeed,
				TopologySettings.GetInstance(),
				EngineSettings.GetInstance()
			);
			newState.SaveToFile(fileName);
		}

		private static void ListAlgorithms(string consoleParameter, ConsoleParameters parameters)
		{
			List<ISchedulerAlgorithm> algorithms = AlgorihtmsContainer.GetAvailableAlgorithms();
			if (!algorithms.Any())
				System.Console.Out.WriteLine("There are no available algorithms.");
			else foreach (ISchedulerAlgorithm algorithm in algorithms)
				System.Console.Out.WriteLine(algorithm.Name);
		}
	}
}