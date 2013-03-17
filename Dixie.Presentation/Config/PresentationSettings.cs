﻿using Configuration;
using System;

namespace Dixie.Presentation
{
	[Serializable]
	[Configuration("dixie.Presentation", false)]
	public class PresentationSettings
	{
		public static PresentationSettings GetInstance()
		{
			return Configuration<PresentationSettings>.Get();
		}

		public bool EnableTopologyRendering = true;
		public bool EnableTasksRendering = true;
	}
}