using System;

namespace Dixie.Presentation
{
	[Serializable]
	internal class PresentationSettings
	{
		public static PresentationSettings GetInstance()
		{
			return Instance;
		}

		public static void ToggleRendering()
		{
			Instance = new PresentationSettings
			{
				EnableTopologyRendering = !Instance.EnableTopologyRendering,
				EnableTasksRendering = !Instance.EnableTasksRendering,
			};
		}

		public bool EnableTopologyRendering = true;
		public bool EnableTasksRendering = true;

		private static PresentationSettings Instance = new PresentationSettings();
	}
}