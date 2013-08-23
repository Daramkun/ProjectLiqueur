using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Daramkun.Liqueur.Platforms
{
	public class Launcher : ILauncher
	{
		public PlatformInformation PlatformInformation
		{
			get
			{
				return new PlatformInformation ()
				{
					PlatformType = PlatformType.WindowsPhone,
					PlatformVersion = Environment.OSVersion.Version,
				};
			}
		}

		public bool IsInitialized
		{
			get { throw new NotImplementedException (); }
		}

		public Launcher ( Game game )
		{
			
		}

		public void LauncherInitialize ( out IWindow window, out Graphics.IGraphicsDevice graphicsDevice, out Audio.IAudioDevice audioDevice )
		{
			throw new NotImplementedException ();
		}

		public void LauncherRun ( LauncherArgument args )
		{
			throw new NotImplementedException ();
		}

		public void LauncherFinalize ( IWindow window, Graphics.IGraphicsDevice graphicsDevice, Audio.IAudioDevice audioDevice )
		{
			throw new NotImplementedException ();
		}
	}
}
