using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Medias;
using System.Net.NetworkInformation;
using Windows.UI.Core;
using Windows.ApplicationModel.Core;
using Windows.System.Profile;

namespace Daramkun.Liqueur.Platforms
{
	public class Launcher : ILauncher
	{
		private string GetHardwareId ()
		{
			var token = HardwareIdentification.GetPackageSpecificToken ( null );
			var hardwareId = token.Id;
			var dataReader = Windows.Storage.Streams.DataReader.FromBuffer ( hardwareId );

			byte [] bytes = new byte [ hardwareId.Length ];
			dataReader.ReadBytes ( bytes );

			return BitConverter.ToString ( bytes );
		}  

		public PlatformInformation PlatformInformation
		{
			get
			{
				return new PlatformInformation ()
				{
					Platform = Platforms.Platform.WindowsRT,
					PlatformVersion = new Version ( 6, 2, 0, 0 ),
					Is64BitPlatform = BooleanState.Unknown,
					Is64BitProcess = BooleanState.Unknown,
					UserName = Windows.System.UserProfile.UserInformation.GetDisplayNameAsync ().GetResults (),
					MachineUniqueIdentifier = GetHardwareId (),
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
			//SoundContentLoader.SoundType = typeof ( SoundPlayer );
		}

		public void LauncherFinalize ( IWindow window, IRenderer renderer )
		{
			( renderer as Renderer ).Dispose ();
			( window as Window ).Dispose ();
		}

		private class WindowFactory : IFrameworkViewSource
		{
			public IFrameworkView CreateView ()
			{
				return LiqueurSystem.Window as Window;
			}
		}

		public void Run ( Action initialize, Action updateLogic, Action drawLogic, Action resize, Action activated, Action deactivated, params object [] arguments )
		{
			( LiqueurSystem.Window as Window ).InitializeWindow = ( CoreWindow window ) =>
			{
				window.SizeChanged += ( CoreWindow sender, WindowSizeChangedEventArgs e ) => { resize (); };
				window.VisibilityChanged += ( CoreWindow sender, VisibilityChangedEventArgs e ) =>
				{
					if ( e.Visible ) activated ();
					else deactivated ();
				};
				initialize ();
				Initialized = true;
			};

			( LiqueurSystem.Window as Window ).UpdateLogic += updateLogic;
			( LiqueurSystem.Window as Window ).DrawLogic += drawLogic;

			CoreApplication.Run ( new WindowFactory () );
		}
	}
}
