using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Platforms
{
	public struct LauncherArgument
	{
		public Action Initialize { get; internal set; }
		public Action UpdateLogic { get; internal set; }
		public Action DrawLogic { get; internal set; }

		public Action Resize { get; internal set; }
		public Action Activated { get; internal set; }
		public Action Deactivated { get; internal set; }
	}

	public enum PlatformType
	{
		Unknown = 0,

		WindowsNT = 1,
		Unix = 2,
		OSX = 3,
		Cosmos = 4,

		WindowsPhone = 12,
		WindowsRT = 13,
		Android = 14,
		iOS = 15,
		Blackberry = 16,

		Xbox360 = 25,
		PlaystationMobile = 26,
		OUYA = 27,
	}

	public struct PlatformInformation
	{
		public PlatformType PlatformType { get; set; }
		public Version PlatformVersion { get; set; }

		public string UserName { get; set; }
		public string MachineUniqueIdentifier { get; set; }
	}

	public interface ILauncher
	{
		bool IsInitialized { get; }

		PlatformInformation PlatformInformation { get; }

		void InvokeInMainThread ( Action action, bool waitForEndOfMethod = true );

		void LauncherInitialize ( out IWindow window, out IGraphicsDevice graphicsDevice, out IAudioDevice audioDevice );
		void LauncherRun ( LauncherArgument args );
		void LauncherFinalize ( IWindow window, IGraphicsDevice graphicsDevice, IAudioDevice audioDevice );
	}
}
