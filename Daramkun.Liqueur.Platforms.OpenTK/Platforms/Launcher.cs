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
#if OPENTK
					Platform = ( os.Platform == PlatformID.Win32NT ) ? Platform.WindowsNT :
								( os.Platform == PlatformID.Xbox ) ? Platform.Xbox360 :
								( os.Platform == PlatformID.Unix ) ? Platform.Unix :
								( os.Platform == PlatformID.MacOSX ) ? Platform.OSX :
								Platform.Unknown,
#elif XNA
#if WINDOWS_PHONE
					Platform = Platforms.Platform.WindowsPhone,
#endif
#endif
					PlatformVersion = os.Version,

#if OPENTK
					Is64BitPlatform = Environment.Is64BitOperatingSystem ? BooleanState.True : BooleanState.False,
					Is64BitProcess = Environment.Is64BitProcess ? BooleanState.True : BooleanState.False,
#endif
#if !WINDOWS_PHONE
					UserName = Environment.UserName,
					MachineUniqueIdentifier = NetworkInterface.GetAllNetworkInterfaces () [ 0 ].GetPhysicalAddress ().ToString (),
#else
					MachineUniqueIdentifier = NetworkInterface.GetInternetInterface ().ToString (),
#endif
				};
			}
		}

		public void LauncherInitialize ( out IWindow window, out IRenderer renderer )
		{
			window = new Window ();
			renderer = new Renderer ( window as Window );

			ImageContentLoader.ImageType = typeof ( Image );
			SoundContentLoader.SoundType = typeof ( SoundPlayer );
		}

		public void LauncherFinalize ( IWindow window, IRenderer renderer )
		{
#if OPENTK
			updateThread.Abort ();
#endif
			( renderer as Renderer ).Dispose ();
			( window as Window ).Dispose ();
		}

		public void Run ( Action updateLogic, Action drawLogic, Action resize, Action activated, Action deactivated, params object [] arguments )
		{
#if OPENTK
			GameWindow window = LiqueurSystem.Window.Handle as GameWindow;
			window.Resize += ( object sender, EventArgs e ) => { resize (); };
			window.FocusedChanged += ( object sender, EventArgs e ) =>
			{
				if ( window.Focused ) activated ();
				else deactivated ();
			};
			window.RenderFrame += ( object sender, FrameEventArgs e ) => { drawLogic (); };
			//window.UpdateFrame += ( object sender, FrameEventArgs e ) => { updateLogic (); };
			updateThread = new Thread ( () => { while ( true ) { updateLogic (); Thread.Sleep ( 1 ); } } );
			updateThread.Start ();
			window.Run ();
#elif XNA
			Window.InternalGame game = LiqueurSystem.Window.Handle as Window.InternalGame;
#if !WINDOWS_PHONE
			game.Window.ClientSizeChanged += ( object sender, EventArgs e ) => { resize (); };
			game.Activated += ( object sender, EventArgs e ) => { activated (); };
			game.Deactivated += ( object sender, EventArgs e ) => { deactivated (); };
#endif

			game.Run ();
#endif
		}
	}
}
