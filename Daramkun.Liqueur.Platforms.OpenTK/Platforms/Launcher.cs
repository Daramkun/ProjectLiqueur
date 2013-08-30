using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Logging;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Daramkun.Liqueur.Platforms
{
	public class Launcher : ILauncher
	{
#if OPENGL2
		private const int SupportOpenGLVersion = 2;
#else
		private const int SupportOpenGLVersion = 3;
#endif

		Thread updateThread;
		IGraphicsContext updateContext;

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

		public Launcher ()
		{
			IsInitialized = false;
		}

		public void LauncherInitialize ( out IWindow window, out IGraphicsDevice graphicsDevice, out IAudioDevice audioDevice )
		{
			window = new Window ();
			graphicsDevice = new GraphicsDevice ( window );
			audioDevice = new AudioDevice ( window );

			IsInitialized = true;
		}

		public void LauncherRun ( LauncherArgument args )
		{
			GraphicsContext.ShareContexts = true;
			GameWindow window = LiqueurSystem.Window.Handle as GameWindow;

			if ( int.Parse ( GL.GetString ( StringName.Version ) [ 0 ].ToString () ) <= SupportOpenGLVersion - 1 )
				throw new PlatformNotSupportedException (
					string.Format ( "Platform is not support OpenGL {0}.0 (Support maximum OpenGL Version: {1})",
					SupportOpenGLVersion, GL.GetString ( StringName.Version ) )
				);
			
			window.Resize += ( object sender, EventArgs e ) => { args.Resize (); };
			window.FocusedChanged += ( object sender, EventArgs e ) =>
			{
				if ( window.Focused )
				{
					if ( LiqueurSystem.GraphicsDevice.FullscreenMode )
						OpenTK.DisplayDevice.Default.ChangeResolution ( OpenTK.DisplayDevice.Default.SelectResolution ( ( int )
							LiqueurSystem.GraphicsDevice.ScreenSize.X, ( int ) LiqueurSystem.GraphicsDevice.ScreenSize.Y, 32, 60 ) );
					args.Activated ();
				}
				else
				{
					args.Deactivated ();
					OpenTK.DisplayDevice.Default.RestoreResolution ();
					OpenTK.DisplayDevice.Default.ChangeResolution ( ( LiqueurSystem.GraphicsDevice as GraphicsDevice ).originalResolution );
				}
			};
			window.Context.SwapInterval = 0;

			window.Load += ( object sender, EventArgs e ) =>
			{
				args.Initialize ();
			};
			window.RenderFrame += ( object sender, FrameEventArgs e ) =>
			{
				if ( !window.Context.IsCurrent )
				window.Context.MakeCurrent ( window.WindowInfo );
				args.DrawLogic ();
			};
			updateThread = new Thread ( () =>
			{
				updateContext = new GraphicsContext ( GraphicsMode.Default, window.WindowInfo );
				while ( true )
				{
					try
					{
						if ( !updateContext.IsCurrent )
							updateContext.MakeCurrent ( window.WindowInfo );
					}
					catch ( Exception e )
					{
						Logger.Write ( LogLevel.Level1, "{0}", e );
					}
					args.UpdateLogic ();
					Thread.Sleep ( 1 );
				}
			} );
			updateThread.Start ();
			window.Run ();
		}

		public void LauncherFinalize ( IWindow window, IGraphicsDevice graphicsDevice, IAudioDevice audioDevice )
		{
			updateThread.Abort ();
			updateThread = null;
			if ( audioDevice != null )
				audioDevice.Dispose ();
			updateContext.Dispose ();
			graphicsDevice.Dispose ();
			window.Dispose ();
		}
	}
}
