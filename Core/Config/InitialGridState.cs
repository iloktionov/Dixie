namespace Dixie.Core
{
	public class InitialGridState
	{
		public InitialGridState(Topology topology, int randomSeed, TopologySettings topologySettings, EngineSettings engineSettings)
		{
			EngineSettings = engineSettings;
			TopologySettings = topologySettings;
			Topology = topology;
			RandomSeed = randomSeed;
		}

		public Topology Topology { get; private set; }
		public int RandomSeed { get; private set; }
		public TopologySettings TopologySettings { get; private set; }
		public EngineSettings EngineSettings { get; private set; }
	}
}