using System;
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
			MapActionToNamedParameter("--test", PerformTest, 5);
		}

		private static void GenerateInitialState(string key, ConsoleParameters parameters)
		{
			int nodesCount = Int32.Parse(parameters.GetValue(key, 0));
			string outputFilename = parameters.GetValue(key, 1);
			InitialGridState.GenerateNew(nodesCount).SaveToFile(outputFilename);
		}

		private static void UpdateStateSettings(string key, ConsoleParameters parameters)
		{
			string fileName = parameters.GetValue(key, 0);
			InitialGridState deserializedState = InitialGridState.ReadFromFile(fileName);
			var newState = new InitialGridState(
				deserializedState.Topology, 
				deserializedState.RandomSeed,
				TopologySettings.GetInstance(),
				EngineSettings.GetInstance()
			);
			newState.SaveToFile(fileName);
		}

		private static void ListAlgorithms(string key, ConsoleParameters parameters)
		{
			List<ISchedulerAlgorithm> algorithms = AlgorihtmsContainer.GetAvailableAlgorithms();
			if (!algorithms.Any())
				System.Console.Out.WriteLine("There are no available algorithms.");
			else foreach (ISchedulerAlgorithm algorithm in algorithms)
				System.Console.Out.WriteLine(algorithm.Name);
		}

		private static void PerformTest(string key, ConsoleParameters parameters)
		{
			InitialGridState initialState = InitialGridState.ReadFromFile(parameters.GetValue(key, 0));
			TimeSpan testDuration = TimeSpanParser.Parse(parameters.GetValue(key, 1));
			TimeSpan resultsPeriod = TimeSpanParser.Parse(parameters.GetValue(key, 2));
			string outputFileName = parameters.GetValue(key, 3);

			ILog log = new ColorConsoleLog();
			List<ISchedulerAlgorithm> availableAlgorithms = AlgorihtmsContainer.GetAvailableAlgorithms();
			var testedAlgorithms = new List<ISchedulerAlgorithm>();
			for (int i = 4; i < parameters.GetValuesCount(key); i++)
			{
				string algorithmName = parameters.GetValue(key, i);
				ISchedulerAlgorithm algorithm = availableAlgorithms.FirstOrDefault(algo => String.Equals(algo.Name, algorithmName, StringComparison.InvariantCultureIgnoreCase));
				if (algorithm == null)
				{
					log.Error("Algorithm with name {0} was not found.", algorithmName);
					return;
				}
				testedAlgorithms.Add(algorithm);
			}

			var engine = new Engine(initialState, log);
			ComparisonTestResult testResult = engine.TestAlgorithms(testedAlgorithms, testDuration, resultsPeriod);
			log.Info("Test result: {0}{1}", Environment.NewLine, testResult);
			testResult.SaveToFile(outputFileName);
		}
	}
}