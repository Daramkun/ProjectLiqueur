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
					PlatformType = PlatformType.iOS,
					PlatformVersion = Environment.OSVersion.Version,
					MachineUniqueIdentifier = NetworkInterface.GetAllNetworkInterfaces () [ 0 ].GetPhysicalAddress ().ToString ()
				};
			}
		}

		System.Drawing.RectangleF frame;
		public Launcher ( System.Drawing.RectangleF frame )
		{
			IsInitialized = false;
			this.frame = frame;
		}

		public void LauncherInitialize ( out IWindow window, out IGraphicsDevice graphicsDevice, out IAudioDevice audioDevice )
		{
			window = new Window ( frame );
			graphicsDevice = new GraphicsDevice ( window );
			audioDevice = new AudioDevice ( window );

			IsInitialized = true;
		}

		public void LauncherRun ( LauncherArgument args )
		{
			GraphicsContext.ShareContexts = true;
			OpenTK.Platform.iPhoneOS.iPhoneOSGameView gameView = LiqueurSystem.Window.Handle as OpenTK.Platform.iPhoneOS.iPhoneOSGameView;

			gameView.Resize += ( object sender, EventArgs e ) => { args.Resize (); };
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

