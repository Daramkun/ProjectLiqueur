using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Inputs.RawDevices;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Scenes;

namespace Daramkun.Liqueur
{
	public static class LiqueurSystem
	{
		public static IWindow Window { get; private set; }
		public static IRenderer Renderer { get; private set; }

		public static FrameScene FrameScene { get; private set; }

		public static RawKeyboard Keyboard { get; set; }
		public static RawMouse Mouse { get; set; }
		public static GamePadCollection GamePads { get; set; }
		public static RawTouchPanel TouchPanel { get; set; }
		public static RawAccelerometer Accelerometer { get; set; }

		public static EventHandler Initialize, Finalize;
		public static EventHandler<GameTimeEventArgs> Update, Draw;
		public static EventHandler Resize, Activated, Deactivated;

		public static TimeSpan FixedUpdateTimeStep { get; set; }
		public static TimeSpan FixedDrawTimeStep { get; set; }
		private static GameTime updateGameTime = new GameTime (),
								drawGameTime = new GameTime ();

		private static ILauncher Launcher { get; set; }
		public static PlatformInformation PlatformInformation { get { return Launcher.PlatformInformation; } }

		public static CultureInfo CurrentCulture { get; set; }

		static LiqueurSystem ()
		{
			CurrentCulture = CultureInfo.CurrentCulture;
			GamePads = new GamePadCollection ();
			FixedUpdateTimeStep = new TimeSpan ();
			FixedDrawTimeStep = new TimeSpan ();
		}

		public static void Run ( ILauncher launcher, Scene firstScene, params object [] arguments )
		{
			Launcher = launcher;

			IWindow window; IRenderer renderer;
			Launcher.LauncherInitialize ( out window, out renderer );
			Window = window; Renderer = renderer;

			if ( Initialize != null )
				Initialize ( null, EventArgs.Empty );
			FrameScene = new FrameScene ( firstScene );
			FrameScene.OnInitialize ();

			TimeSpan elapsedUpdateTimeStep = new TimeSpan (),
				elapsedDrawTimeStep = new TimeSpan ();
			TimeSpan lastUpdateTimeStep = TimeSpan.FromMilliseconds ( Environment.TickCount ),
				lastDrawTimeStep = TimeSpan.FromMilliseconds ( Environment.TickCount );

			Launcher.Run (
				() =>
				{
					if ( elapsedUpdateTimeStep >= FixedUpdateTimeStep )
					{
						updateGameTime.Update ();
						FrameScene.OnUpdate ( updateGameTime );
						if ( Update != null )
							Update ( null, new GameTimeEventArgs ( updateGameTime ) );
						elapsedUpdateTimeStep -= FixedUpdateTimeStep;
					}
					else
					{
						TimeSpan temp = TimeSpan.FromMilliseconds ( Environment.TickCount );
						elapsedUpdateTimeStep += ( temp - lastUpdateTimeStep );
						lastUpdateTimeStep = temp;
					}
				},
				() =>
				{
					if ( elapsedDrawTimeStep >= FixedDrawTimeStep )
					{
						drawGameTime.Update ();
						FrameScene.OnDraw ( drawGameTime );
						if ( Draw != null )
							Draw ( null, new GameTimeEventArgs ( drawGameTime ) );
						elapsedDrawTimeStep -= FixedDrawTimeStep;
					}
					else
					{
						TimeSpan temp = TimeSpan.FromMilliseconds ( Environment.TickCount );
						elapsedDrawTimeStep += ( temp - lastDrawTimeStep );
						lastDrawTimeStep = temp;
					}
				},
				() =>
				{
					FrameScene.OnResize ();
					if ( Resize != null )
						Resize ( null, EventArgs.Empty );
				},
				() =>
				{
					FrameScene.OnActivated ();
					if ( Activated != null )
						Activated ( null, EventArgs.Empty );
				},
				() =>
				{
					FrameScene.OnDeactivated ();
					if ( Deactivated != null )
						Deactivated ( null, EventArgs.Empty );
				},
				arguments
			);

			FrameScene.OnFinalize ();
			if ( Finalize != null )
				Finalize ( null, EventArgs.Empty );
			Launcher.LauncherFinalize ( Window, Renderer );
		}
	}
}
