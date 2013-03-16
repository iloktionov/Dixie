using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dixie.Core
{
	[Serializable]
	public partial class Node : INode, IDeserializationCallback
	{
		internal Node(double performance, double failureProbability, NodeFailurePattern failurePattern)
		{
			Preconditions.CheckArgument(performance > 0, "performance", "Must be positive.");
			Preconditions.CheckArgument(failureProbability >= 0 && failureProbability <= 1, "failureProbability", "Must be in [0; 1].");
			Performance = performance;
			FailureProbability = failureProbability;
			this.failurePattern = failurePattern;
			Id = Guid.NewGuid();
			workBuffer = new WorkBuffer();
			LastHBTimestamp = TimeSpan.MinValue;
			syncObject = new object();
		}

		public Node(double performance, double failureProbability)
			: this (performance, failureProbability, NodeFailurePattern.CreateDefaults()) { }

		public NodeState GetState()
		{
			lock (syncObject)
				return new NodeState(Id, Performance, FailureProbability, workBuffer.Size);
		}

		internal HeartBeatMessage GetHeartBeatMessage()
		{
			lock (syncObject)
			{
				List<Guid> completedTasks = workBuffer.PopCompletedOrNull();
				return new HeartBeatMessage(Id, Performance, workBuffer.Size, completedTasks);
			}
		}

		internal void HandleHeartBeatResponse(HeartBeatResponse response)
		{
			lock (syncObject)
			{
				if (response.Tasks != null)
					foreach (Task task in response.Tasks)
						workBuffer.PutTask(task.Id, GetCalculationTime(task));
			}
		}

		internal NodeFailureType GetFailureType(Random random)
		{
			return failurePattern.DetermineFailureType(random);
		}

		public bool IsComputing()
		{
			lock (syncObject)
				return workBuffer.IsComputing();
		}

		public void StopComputing()
		{
			lock (syncObject)
				workBuffer.StopComputing();
		}

		public void ResumeComputing()
		{
			lock (syncObject)
				workBuffer.ResumeComputing();
		}

		public Guid Id { get; private set; }

		/// <summary>
		/// Measured in work/msec.
		/// </summary>
		public Double Performance { get; private set; }

		/// <summary>
		/// In [0;1] interval.
		/// </summary>
		public Double FailureProbability { get; private set; }

		[NonSerialized]
		internal TimeSpan LastHBTimestamp;

		#region Equality members
		protected bool Equals(Node other)
		{
			return Id.Equals(other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != GetType()) return false;
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
			LastHBTimestamp = TimeSpan.MinValue;
			syncObject = new object();
		} 
		#endregion

		private TimeSpan GetCalculationTime(Task task)
		{
			return TimeSpan.FromMilliseconds(task.Volume / Performance);
		}

		private readonly NodeFailurePattern failurePattern;
		[NonSerialized] private WorkBuffer workBuffer;
		[NonSerialized] private object syncObject;
	}
}