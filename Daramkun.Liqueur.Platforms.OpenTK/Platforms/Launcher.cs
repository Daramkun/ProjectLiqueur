using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Medias;
#if WINDOWS_PHONE
using Microsoft.Phone.Net.NetworkInformation;
#endif
#if OPENTK
using OpenTK;
using System.Net.NetworkInformation;
#endif

namespace Daramkun.Liqueur.Platforms
{
	public class Launcher : ILauncher
	{
#if OPENTK
		Thread updateThread;
#endif

		public PlatformInformation PlatformInformation
		{
			get
			{
				OperatingSystem os = Environment.OSVersion;

				return new PlatformInformation ()
				{
					Platform = ( os.Platform == PlatformID.Win32NT ) ? Platform.WindowsNT :
								( os.Platform == PlatformID.Xbox ) ? Platform.Xbox360 :
								( os.Platform == PlatformID.Unix ) ? Platform.Unix :
								( os.Platform == PlatformID.MacOSX ) ? Platform.OSX :
								Platform.Unknown,
					PlatformVersion = os.Version,

					Is64BitPlatform = Environment.Is64BitOperatingSystem ? BooleanState.True : BooleanState.False,
					Is64BitProcess = Environment.Is64BitProcess ? BooleanState.True : BooleanState.False,

					UserName = Environment.UserName,
					MachineUniqueIdentifier = NetworkInterface.GetAllNetworkInterfaces () [ 0 ].GetPhysicalAddress ().ToString (),
				};
			}
		}

		public bool Initialized { get; private set; }

		public Launcher ()
		{
			Initialized = false;
		}

		public void LauncherInitialize ( out IWindow window, out IRenderer renderer )
		{
			window = new Window ();
			renderer = new Renderer ( window as Window );

			ImageContentLoader.ImageType = typeof ( Image );
			SoundContentLoader.SoundType = typeof ( SoundPlayer );

			Initialized = true;
		}

		public void LauncherFinalize ( IWindow window, IRenderer renderer )
		{
			updateThread.Abort ();
			( renderer as Renderer ).Dispose ();
			( window as Window ).Dispose ();
		}

		public void Run ( Action initialize, Action updateLogic, Action drawLogic, Action resize, Action activated, Action deactivated, params object [] arguments )
		{
			GameWindow window = LiqueurSystem.Window.Handle as GameWindow;
			window.Resize += ( object sender, EventArgs e ) => { resize (); };
			window.FocusedChanged += ( object sender, EventArgs e ) =>
			{
				if ( window.Focused ) activated ();
				else deactivated ();
			};
			initialize ();
			window.RenderFrame += ( object sender, FrameEventArgs e ) => { drawLogic (); };
			updateThread = new Thread ( () => { while ( true ) { updateLogic (); Thread.Sleep ( 1 ); } } );
			updateThread.Start ();
			window.Run ();
		}
	}
}
