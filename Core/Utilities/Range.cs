using System;

namespace Dixie.Core
{
	public class Range<T> where T: IComparable<T>
	{
		public Range(T from, T to)
		{
			Preconditions.CheckArgument(to.CompareTo(from) >= 0, "to", "Range end element is less than range start element!");
			From = from;
			To = to;
		}

		public T From { get; private set; }
		public T To { get; private set; }
	}
}