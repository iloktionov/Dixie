using System;

namespace Dixie.Core
{
	/// <summary>
	/// Тип ноды, обозначающий место Мастера в топологии грида.  
	/// </summary>
	[Serializable]
	internal class MasterFakeNode : INode
	{
		public Guid Id
		{
			get { return Guid.Empty; }
		}

		public double Performance
		{
			get { return 0; }
		}

		public double FailureProbability
		{
			get { return 0; }
		}
	}
}