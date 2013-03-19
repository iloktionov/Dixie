using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Reflection;

namespace Dixie.Core
{
	internal class AlgorithmsContainer
	{
		static AlgorithmsContainer()
		{
			instance = new AlgorithmsContainer();
		}

		public static void Initialize()
		{
			
		}

		public static List<ISchedulerAlgorithm> GetAvailableAlgorithms()
		{
			return instance.GetAlgorithms();
		}

		public const string ExtensionsDirectory = "Algorithms\\";

		private AlgorithmsContainer()
		{
			var catalog = new AggregateCatalog();
			catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
			string extensionsDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ExtensionsDirectory);
			if (Directory.Exists(extensionsDirectoryPath))
				catalog.Catalogs.Add(new DirectoryCatalog(extensionsDirectoryPath));
			var container = new CompositionContainer(catalog);
			container.ComposeParts(this);
		}

		private List<ISchedulerAlgorithm> GetAlgorithms()
		{
			return algorithms;
		}

		[ImportMany(typeof(ISchedulerAlgorithm))]
		private List<ISchedulerAlgorithm> algorithms;
		private static readonly AlgorithmsContainer instance;
	}
}