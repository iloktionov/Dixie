using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Dixie.Core
{
	[Serializable]
	internal partial class Node : INode, IDeserializationCallback
	{
		private Node(Guid id, double performance, double failureProbability, NodeFailurePattern failurePattern)
		{
			Preconditions.CheckArgument(performance > 0, "performance", "Must be positive.");
			Preconditions.CheckArgument(failureProbability >= 0 && failureProbability <= 1, "failureProbability", "Must be in [0; 1].");
			Id = id;
			Performance = performance;
			FailureProbability = failureProbability;
			this.failurePattern = failurePattern;
			workBuffer = new WorkBuffer();
			LastHBTimestamp = TimeSpan.MinValue;
			syncObject = new object();
		}

		internal Node(double performance, double failureProbability, NodeFailurePattern failurePattern)
			: this (Guid.NewGuid(), performance, failureProbability, failurePattern) { }

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

		internal bool IsComputing()
		{
			lock (syncObject)
				return workBuffer.IsComputing();
		}

		internal void StopComputing()
		{
			lock (syncObject)
				workBuffer.StopComputing();
		}

		internal void ResumeComputing()
		{
			lock (syncObject)
				workBuffer.ResumeComputing();
		}

		internal Node Clone()
		{
			return new Node(Id, Performance, FailureProbability, failurePattern);
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