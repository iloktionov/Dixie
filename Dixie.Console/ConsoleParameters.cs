using System.Collections.Generic;

namespace Dixie.Console
{
	internal class ConsoleParameters
	{
		public ConsoleParameters(IEnumerable<string> arguments)
		{
			if (arguments == null)
				return;
			string current = string.Empty;
			foreach (string argument in arguments)
			{
				if (argument.StartsWith("-"))
				{
					namedParams.Add(argument, new List<string>());
					current = argument;
				}
				else
				{
					if (string.IsNullOrEmpty(current))
						mainParams.Add(argument);
					else
						namedParams[current].Add(argument);
				}
			}
		}

		public bool NoParams()
		{
			return namedParams.Count == 0 && mainParams.Count == 0;
		}

		public bool HasParam(string param)
		{
			return namedParams.ContainsKey(param);
		}

		public bool HasParamWithValue(string param)
		{
			return namedParams.ContainsKey(param) && namedParams[param].Count > 0;
		}

		public string GetValue(string name)
		{
			return GetValue(name, 0);
		}

		public string GetValue(string name, int index)
		{
			return namedParams[name][index];
		}

		public List<string> GetValues(string name)
		{
			return namedParams[name];
		}

		public string GetMainValue(int index)
		{
			return mainParams[index];
		}

		public List<string> GetMainValues()
		{
			return mainParams;
		}

		public int GetValuesCount(string name)
		{
			return namedParams[name].Count;
		}

		public int NamedParamsCount { get { return namedParams.Count; } }
		public int MainParamsCount { get { return mainParams.Count; } }

		private readonly Dictionary<string, List<string>> namedParams = new Dictionary<string, List<string>>();
		private readonly List<string> mainParams = new List<string>();
	}
}