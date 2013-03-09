using System;
using System.Collections.Generic;
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

		public HeartBeatMessage GetHeartBeatMessage()
		{
			List<Guid> completedTasks = workBuffer.PopCompletedOrNull(); 
			return new HeartBeatMessage(Id, workBuffer.Size, completedTasks);
		}

		public void HandleHeartBeatResponse(HeartBeatResponse response)
		{
			if (response.Tasks != null)
				foreach (ComputationalTask task in response.Tasks)
					workBuffer.PutTask(task.Id, GetCalculationTime(task));
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

		private TimeSpan GetCalculationTime(ComputationalTask task)
		{
			return TimeSpan.FromMilliseconds(task.Volume / Performance);
		}

		[NonSerialized]
		private WorkBuffer workBuffer;
	}
}