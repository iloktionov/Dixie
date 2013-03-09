using System;
using System.Runtime.Serialization;

namespace Dixie.Core
{
	[Serializable]
	public partial class Node : INode, IDeserializationCallback
	{
		public Node(double performance, double failureProbability)
		{
			Performance = performance;
			FailureProbability = failureProbability;
			Id = Guid.NewGuid();
			workBuffer = new WorkBuffer();
		}

		public Guid Id { get; private set; }
		public Double Performance { get; private set; }
		public Double FailureProbability { get; private set; }

		#region Equality members
		protected bool Equals(Node other)
		{
			return Id.Equals(other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Node)obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		} 
		#endregion

		#region Serialization
		public void OnDeserialization(object sender)
		{
			workBuffer = new WorkBuffer();
		} 
		#endregion

		[NonSerialized]
		private WorkBuffer workBuffer;
	}
}