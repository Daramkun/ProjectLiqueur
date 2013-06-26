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
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
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
								( os.Platform == PlatformID.Unix ) ? Platform.Unix :
								( os.Platform == PlatformID.MacOSX ) ? Platform.OSX :
								Platform.Unknown,
					PlatformVersion = os.Version,

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

			Texture2DContentLoader.ImageType = typeof ( Texture2D );
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
			OpenTK.Graphics.GraphicsContext.ShareContexts = true;
			GameWindow window = LiqueurSystem.Window.Handle as GameWindow;

			if ( int.Parse ( GL.GetString ( StringName.Version ) [ 0 ].ToString () ) <= 2 )
				throw new PlatformNotSupportedException ( 
					"Project Liqueur OpenTK Platform Extension is not support OpenGL 2.0 or lower." );

			window.Resize += ( object sender, EventArgs e ) => { resize (); };
			window.FocusedChanged += ( object sender, EventArgs e ) =>
			{
				if ( window.Focused ) activated ();
				else deactivated ();
			};
			window.Context.SwapInterval = 0;

			initialize ();
			window.RenderFrame += ( object sender, FrameEventArgs e ) =>
			{
				window.Context.MakeCurrent ( window.WindowInfo );
				drawLogic ();
			};
			updateThread = new Thread ( () =>
			{
				GraphicsContext context = new GraphicsContext ( GraphicsMode.Default,
					window.WindowInfo, 4, 0, GraphicsContextFlags.Default );
				while ( true )
				{
					context.MakeCurrent ( window.WindowInfo );
					updateLogic ();
					Thread.Sleep ( 1 );
				}
			} );
			updateThread.Start ();
			window.Run ();
		}
	}
}
