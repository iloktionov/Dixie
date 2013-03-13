using System;

namespace Dixie.Core
{
	public class Task
	{
		public Task(double volume)
		{
			Volume = volume;
			Id = Guid.NewGuid();
		}

		public Guid Id { get; private set; }
		public Double Volume { get; private set; }

		#region Equality members
		protected bool Equals(Task other)
		{
			return Id.Equals(other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((Task)obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		} 
		#endregion
	}
}