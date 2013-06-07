﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Platforms
{
	public interface ILauncher
	{
		PlatformInformation PlatformInformation { get; }

		void LauncherInitialize ( out IWindow window, out IRenderer renderer );
		void LauncherFinalize ( IWindow window, IRenderer renderer );
		void Run ( Action updateLogic, Action drawLogic,
			Action resize, Action activated, Action deactivated );
	}
}