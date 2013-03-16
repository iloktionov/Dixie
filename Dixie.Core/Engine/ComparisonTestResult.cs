using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Dixie.Core
{
	[Serializable]
	public class ComparisonTestResult
	{
		public ComparisonTestResult()
		{
			AlgorithmResults = new Dictionary<string, AlgorithmTestResult>();
		}

		public string GetWinnerOrNull()
		{
			return AlgorithmResults.Count > 0
				? AlgorithmResults.MaxBy(pair => pair.Value.TotalWorkDone).Key
				: null;
		}

		internal void AddAlgorithmResult(ISchedulerAlgorithm algorithm, AlgorithmTestResult result)
		{
			AlgorithmResults[algorithm.Name] = result;
		}

		public override string ToString()
		{
			if (AlgorithmResults.Count <= 0)
				return "No results.";
			var builder = new StringBuilder();
			foreach (KeyValuePair<string, AlgorithmTestResult> pair in AlgorithmResults.OrderByDescending(pair => pair.Value.TotalWorkDone))
			{
				builder.AppendLine();
				builder.AppendFormat("{0}: {1}", pair.Key, pair.Value.ToString(true));
			}
			return builder.ToString();
		}

		public void Serialize(Stream stream)
		{
			new BinaryFormatter().Serialize(stream, this);
		}

		public static ComparisonTestResult Deserialize(Stream stream)
		{
			return (ComparisonTestResult) new BinaryFormatter().Deserialize(stream);
		}

		public void SaveToFile(string file)
		{
			using (var stream = new FileStream(file, FileMode.Create, FileAccess.Write))
				Serialize(stream);
		}

		public static ComparisonTestResult ReadFromFile(string file)
		{
			using (var stream = new FileStream(file, FileMode.Open, FileAccess.Read))
				return Deserialize(stream);
		}

		public Dictionary<string, AlgorithmTestResult> AlgorithmResults { get; private set; } 
	}
}