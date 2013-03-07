using System;

namespace Dixie.Core
{
	internal class ComputationalTask
	{
		public ComputationalTask(double volume)
		{
			Volume = volume;
			Id = Guid.NewGuid();
		}

		public Guid Id { get; private set; }
		public Double Volume { get; private set; }

		#region Equality members
		protected bool Equals(ComputationalTask other)
		{
			return Id.Equals(other.Id);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((ComputationalTask)obj);
		}

		public override int GetHashCode()
		{
			return Id.GetHashCode();
		} 
		#endregion
	}
}