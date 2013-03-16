using System;
using Dixie.Core;
using GraphSharp.Algorithms.Layout;
using GraphSharp.Algorithms.Layout.Simple.Hierarchical;
using GraphSharp.Controls;

namespace Dixie.Presentation
{
	internal class DixieGraphLayout : GraphLayout<INode, NetworkLink, DixieGraph>
	{
		public void SetupLayoutParameters()
		{
			IsAnimationEnabled = true;
			AnimationLength = TimeSpan.FromMilliseconds(10);
			LayoutParameters = SugiyamaParameters;
		}

		private static readonly ILayoutParameters SugiyamaParameters = new SugiyamaLayoutParameters
		{
			HorizontalGap = 3,
			VerticalGap = 30,
			DirtyRound = true,
			MinimizeHierarchicalEdgeLong = true,
			BaryCenteringByPosition = true,
			MaxWidth = 1f,
			PositionCalculationMethod = PositionCalculationMethodTypes.IndexBased
		};
	}
}