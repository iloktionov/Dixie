using System;

namespace Dixie.Core
{
	[Serializable]
	internal struct NodeFailureProbabilities
	{
		public NodeFailureProbabilities(double shortTermFailProbability, double longTermFailProbability, double permanentFailProbability)
		{
			Preconditions.CheckArgument(shortTermFailProbability >= 0 && shortTermFailProbability <= 1);
			Preconditions.CheckArgument(longTermFailProbability >= 0 && longTermFailProbability <= 1);
			Preconditions.CheckArgument(permanentFailProbability >= 0 && permanentFailProbability <= 1);
			Preconditions.CheckArgument(Math.Abs(shortTermFailProbability + longTermFailProbability + permanentFailProbability - 1d) < 0.0001d);
			ShortTermFailProbability = shortTermFailProbability;
			LongTermFailProbability = longTermFailProbability;
			PermanentFailProbability = permanentFailProbability;
		}

		public NodeFailureType DetermineFailureType(Random random)
		{
			if (random.TemptProvidence(ShortTermFailProbability))
				return NodeFailureType.ShortTerm;
			return random.TemptProvidence(LongTermFailProbability) ? NodeFailureType.LongTerm : NodeFailureType.Permanent;
		}

		public static NodeFailureProbabilities Generate(Random random)
		{
			double shortTermFailProbability = random.NextDouble();
			double longTermFailProbability = random.NextDouble(0, 1d - shortTermFailProbability);
			double permanentFailProbability = 1d - shortTermFailProbability - longTermFailProbability;
			return new NodeFailureProbabilities(shortTermFailProbability, longTermFailProbability, permanentFailProbability);
		}

		public static NodeFailureProbabilities CreateDefaults()
		{
			return new NodeFailureProbabilities(0.33, 0.33, 1 - 0.33 * 2);
		}

		public override string ToString()
		{
			return String.Format("Short: {0:0.000}%; Long: {1:0.000}%; Perm: {2:0.000}%;",
				ShortTermFailProbability * 100d, LongTermFailProbability * 100d, PermanentFailProbability * 100d);
		}

		public Double ShortTermFailProbability;
		public Double LongTermFailProbability;
		public Double PermanentFailProbability;
	}
}