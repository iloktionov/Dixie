using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Reflection;

namespace Dixie.Core
{
	public class AlgorihtmsContainer
	{
		static AlgorihtmsContainer()
		{
			instance = new AlgorihtmsContainer();
		}

		public static List<ISchedulerAlgorithm> GetAvailableAlgorithms()
		{
			return instance.GetAlgorithms();
		}

		private AlgorihtmsContainer()
		{
			var catalog = new AggregateCatalog();
			catalog.Catalogs.Add(new AssemblyCatalog(Assembly.GetExecutingAssembly()));
			var container = new CompositionContainer(catalog);
			container.ComposeParts(this);
		}

		private List<ISchedulerAlgorithm> GetAlgorithms()
		{
			return algorithms;
		}

		[ImportMany(typeof(ISchedulerAlgorithm))]
		private List<ISchedulerAlgorithm> algorithms;
		private static readonly AlgorihtmsContainer instance;
	}
}