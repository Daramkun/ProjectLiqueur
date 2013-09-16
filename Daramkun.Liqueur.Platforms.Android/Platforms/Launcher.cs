using System;
using Daramkun.Liqueur.Platforms;
using System.Net.NetworkInformation;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Audio;
using OpenTK.Graphics;

namespace Daramkun.Liqueur
{
	public class Launcher : ILauncher
	{
		public bool IsInitialized { get; private set; }

		public PlatformInformation PlatformInformation
		{
			get
			{
				return new PlatformInformation ()
				{
					UserName = Environment.UserName,
					PlatformType = PlatformType.Android,
					PlatformVersion = Environment.OSVersion.Version,
					MachineUniqueIdentifier = NetworkInterface.GetAllNetworkInterfaces () [ 0 ].GetPhysicalAddress ().ToString ()
				};
			}
		}

		Android.Content.Context context;
		public Launcher ( Android.Content.Context context )
		{
			IsInitialized = false;
			this.context = context;
		}

		public void LauncherInitialize ( out IWindow window, out IGraphicsDevice graphicsDevice, out IAudioDevice audioDevice )
		{
			window = new Window ( context );
			graphicsDevice = new GraphicsDevice ( window );
			audioDevice = new AudioDevice ( window );

			( context as Android.App.Activity ).SetContentView ( window.Handle as Android.Views.View );

			IsInitialized = true;
		}

		public void LauncherRun ( LauncherArgument args )
		{
			GraphicsContext.ShareContexts = true;
			OpenTK.Platform.Android.AndroidGameView gameView = LiqueurSystem.Window.Handle as OpenTK.Platform.Android.AndroidGameView;

			gameView.Resize += ( object sender, EventArgs e ) => { args.Resize (); };
			gameView.FocusedChanged += ( object sender, EventArgs e ) =>
			{
				if ( gameView.Focused )
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
			gameView.Load += ( object sender, EventArgs e ) =>
			{
				LiqueurSystem.Window.Title = "Project Liqueur";
				args.Initialize ();
			};
			gameView.UpdateFrame += ( object sender, OpenTK.FrameEventArgs e ) =>
			{
				args.UpdateLogic ();
			};
			gameView.RenderFrame += ( object sender, OpenTK.FrameEventArgs e ) =>
			{
				args.DrawLogic ();
			};
			gameView.Run ();
		}

		public void LauncherFinalize ( IWindow window, IGraphicsDevice graphicsDevice, IAudioDevice audioDevice )
		{
			if ( audioDevice != null )
				audioDevice.Dispose ();
			graphicsDevice.Dispose ();
			window.Dispose ();
		}
	}
}

