using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Dixie.Core
{
	[Serializable]
	public class InitialGridState
	{
		public InitialGridState(Topology topology, int randomSeed, TopologySettings topologySettings, EngineSettings engineSettings)
		{
			EngineSettings = engineSettings;
			TopologySettings = topologySettings;
			Topology = topology;
			RandomSeed = randomSeed;
		}

		public void Serialize(Stream stream)
		{
			new BinaryFormatter().Serialize(stream, this);
		}

		public void SaveToFile(string file)
		{
			using (var stream = new FileStream(file, FileMode.Create, FileAccess.Write))
				Serialize(stream);
		}

		public static InitialGridState Deserialize(Stream stream)
		{
			return (InitialGridState) new BinaryFormatter().Deserialize(stream);
		}

		public static InitialGridState ReadFromFile(string file)
		{
			using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
				return Deserialize(stream);
		}

		public static InitialGridState GenerateNew(int nodesCount, Random random, TopologySettings topologySettings, EngineSettings engineSettings)
		{
			return new InitialGridState(new TopologyBuilder(random).Build(nodesCount), random.Next(), topologySettings, engineSettings);
		}

		public static InitialGridState GenerateNew(int nodesCount, Random random)
		{
			return GenerateNew(nodesCount, random, TopologySettings.GetInstance(), EngineSettings.GetInstance());
		}

		public static InitialGridState GenerateNew(int nodesCount)
		{
			return GenerateNew(nodesCount, new Random());
		}

		public Topology Topology { get; private set; }
		public int RandomSeed { get; private set; }
		public TopologySettings TopologySettings { get; private set; }
		public EngineSettings EngineSettings { get; private set; }
	}
}