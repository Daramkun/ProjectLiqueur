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
		List<object> invokedMethod = new List<object> ();
		Thread thisThread, updateThread;
		bool isMultithread;

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

		public Launcher ( bool isMultithread = false )
		{
			IsInitialized = false;
			this.isMultithread = isMultithread;
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

			thisThread = Thread.CurrentThread;

			string versionString = GL.GetString ( StringName.Version );
			if ( int.Parse ( versionString [ 0 ].ToString () ) < 2 )
				throw new PlatformNotSupportedException (
					string.Format ( "Platform is not support OpenGL {0}.0 (Support maximum OpenGL Version of This platform: {1})",
					2, versionString )
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
				LiqueurSystem.Window.Title = "Project Liqueur";
				args.Initialize ();
			};
			window.UpdateFrame += ( object sender, FrameEventArgs e ) =>
			{
				if ( invokedMethod.Count > 0 )
				{
					foreach ( Action action in invokedMethod.ToArray () )
					{
						action ();
						lock ( invokedMethod ) { invokedMethod.Remove ( action ); }
					}
				}
				if ( !isMultithread )
					args.UpdateLogic ();
			};
			window.RenderFrame += ( object sender, FrameEventArgs e ) =>
			{
				args.DrawLogic ();
				Thread.Sleep ( 0 );
			};
			if ( isMultithread )
			{
				updateThread = new Thread ( () =>
				{
					while ( !window.IsExiting )
					{
						args.UpdateLogic ();
						Thread.Sleep ( 0 );
					}
				} );
				updateThread.Start ();
			}
			window.Run ();
		}

		public void LauncherFinalize ( IWindow window, IGraphicsDevice graphicsDevice, IAudioDevice audioDevice )
		{
			if ( updateThread != null )
				updateThread.Abort ();
			updateThread = null;
			if ( audioDevice != null )
				audioDevice.Dispose ();
			graphicsDevice.Dispose ();
			window.Dispose ();
		}

		public void InvokeInMainThread ( Action action, bool waitForEndOfMethod = true )
		{
			if ( thisThread == Thread.CurrentThread ) { action (); return; }
			lock ( invokedMethod ) { invokedMethod.Add ( action ); }
			while ( waitForEndOfMethod && invokedMethod.Contains ( action ) ) Thread.Sleep ( 1 );
		}
	}
}
