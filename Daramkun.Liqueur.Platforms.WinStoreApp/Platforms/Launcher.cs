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

namespace Daramkun.Liqueur.Platforms
{
	public class Launcher : ILauncher
	{
		public PlatformInformation PlatformInformation
		{
			get
			{
				return new PlatformInformation ()
				{
					Platform = Platforms.Platform.WindowsRT,
					PlatformVersion = new Version(6, 2, 0, 0),
				};
			}
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

		public void Run ( Action updateLogic, Action drawLogic, Action resize, Action activated, Action deactivated, params object [] arguments )
		{
			( LiqueurSystem.Window as Window ).InitializeWindow = ( CoreWindow window ) =>
			{
				window.SizeChanged += ( CoreWindow sender, WindowSizeChangedEventArgs e ) => { resize (); };
				window.VisibilityChanged += ( CoreWindow sender, VisibilityChangedEventArgs e ) =>
				{
					if ( e.Visible ) activated ();
					else deactivated ();
				};
			};

			( LiqueurSystem.Window as Window ).UpdateLogic += updateLogic;
			( LiqueurSystem.Window as Window ).DrawLogic += drawLogic;

			CoreApplication.Run ( new WindowFactory () );
		}
	}
}
