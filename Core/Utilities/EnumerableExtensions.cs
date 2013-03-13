using System;
using System.Collections.Generic;
using System.Linq;

namespace Dixie.Core
{
	internal static class EnumerableExtensions
	{
		public static T MaxBy<T, U>(this IEnumerable<T> enumerable, Func<T, U> selector)
			where U : IComparable<U>
		{
			T result = enumerable.First();
			foreach (T item in enumerable)
			{
				if (selector(item).CompareTo(selector(result)) == 1)
					result = item;
			}
			return result;
		}
	}
}