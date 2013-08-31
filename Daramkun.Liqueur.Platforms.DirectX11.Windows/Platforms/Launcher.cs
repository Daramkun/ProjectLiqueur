using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Platforms
{
	public class Launcher : ILauncher
	{
		Thread updateThread;

		public bool IsInitialized { get; private set; }

		public PlatformInformation PlatformInformation
		{
			get
			{
				OperatingSystem os = Environment.OSVersion;

				return new PlatformInformation ()
				{
					PlatformType = ( os.Platform == PlatformID.Win32NT ) ? PlatformType.WindowsNT :
								PlatformType.Unknown,
					PlatformVersion = os.Version,

					UserName = Environment.UserName,
					MachineUniqueIdentifier = NetworkInterface.GetAllNetworkInterfaces () [ 0 ].GetPhysicalAddress ().ToString (),
				};
			}
		}

		public void LauncherInitialize ( out IWindow window, out IGraphicsDevice graphicsDevice, out IAudioDevice audioDevice )
		{
			window = new Window ();
			graphicsDevice = new GraphicsDevice ( window );
			audioDevice = null;
			IsInitialized = true;
		}

		public void LauncherRun ( LauncherArgument args )
		{
			Form window = LiqueurSystem.Window.Handle as Form;
			window.Resize += ( object sender, EventArgs e ) => { args.Resize (); };
			window.Activated += ( object sender, EventArgs e ) => { args.Activated (); };
			window.Deactivate += ( object sender, EventArgs e ) => { args.Deactivated (); };

			window.Load += ( object sender, EventArgs e ) => { args.Initialize (); };
			updateThread = new Thread ( () =>
			{
				while ( !window.IsHandleCreated ) ;

				while ( true )
				{
					args.UpdateLogic ();
					Thread.Sleep ( 1 );
				}
			} );
			updateThread.Start ();
			window.Show ();
			while ( window.IsHandleCreated )
			{
				args.DrawLogic ();
				LiqueurSystem.Window.DoEvents ();
				Thread.Sleep ( 1 );
			}
		}

		public void LauncherFinalize ( IWindow window, Graphics.IGraphicsDevice graphicsDevice, Audio.IAudioDevice audioDevice )
		{
			graphicsDevice.Dispose ();
			window.Dispose ();
		}
	}
}
