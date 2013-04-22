using System;

namespace Dixie.Core
{
	internal struct SingleExchangeMutation
	{
		public SingleExchangeMutation(int firstIndex, int secondIndex)
		{
			FirstIndex = firstIndex;
			SecondIndex = secondIndex;
		}

		public void Apply(Int32[] solution)
		{
			Int32 tmp = solution[FirstIndex];
			solution[FirstIndex] = solution[SecondIndex];
			solution[SecondIndex] = tmp;
		}

		public static SingleExchangeMutation Generate(Int32[] solution, Random random)
		{
			Int32 index1;
			Int32 index2;
			do
			{
				index1 = random.Next(solution.Length);
				index2 = random.Next(solution.Length);
			}
			while (index1 == index2 || solution[index1] == solution[index2]);
			return new SingleExchangeMutation(index1, index2);
		}

		public Int32 FirstIndex;
		public Int32 SecondIndex;
	}
}