using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;

namespace Daramkun.Liqueur.Platforms
{
	public class Launcher : ILauncher
	{
		public bool IsInitialized { get; private set; }

		public PlatformInformation PlatformInformation
		{
			get
			{
				OperatingSystem os = Environment.OSVersion;

				return new PlatformInformation ()
				{
					PlatformType = ( os.Platform == PlatformID.Win32NT ) ? PlatformType.WindowsNT :
								( os.Platform == PlatformID.Unix ) ? PlatformType.Unix :
								( os.Platform == PlatformID.MacOSX ) ? PlatformType.OSX :
								PlatformType.Unknown,
					PlatformVersion = os.Version,

					UserName = Environment.UserName,
					MachineUniqueIdentifier = NetworkInterface.GetAllNetworkInterfaces () [ 0 ].GetPhysicalAddress ().ToString (),
				};
			}
		}

		public void LauncherInitialize ( out IWindow window, out Graphics.IGraphicsDevice graphicsDevice, out Audio.IAudioDevice audioDevice )
		{
			window = new Window ();
			throw new NotImplementedException ();
		}

		public void LauncherRun ( LauncherArgument args )
		{

		}

		public void LauncherFinalize ( IWindow window, Graphics.IGraphicsDevice graphicsDevice, Audio.IAudioDevice audioDevice )
		{

		}


		public void InvokeInMainThread ( Action action, bool waitForEndOfMethod = true )
		{
			throw new NotImplementedException ();
		}
	}
}
