using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Inputs.RawDevice;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur
{
	public static class LiqueurSystem
	{
		public static IWindow Window { get; private set; }
		public static IGraphicsDevice GraphicsDevice { get; private set; }
		public static IAudioDevice AudioDevice { get; private set; }

		public static KeyboardDevice Keyboard { get; set; }
		public static MouseDevice Mouse { get; set; }
		public static GamePadDevice GamePad { get; set; }
		public static TouchDevice TouchPanel { get; set; }
		public static AccelerometerDevice Accelerometer { get; set; }

		public static CultureInfo CurrentCulture { get; set; }

		public static Node MainNode { get; private set; }

		public static TimeSpan FixedUpdateTimeStep { get; set; }
		public static TimeSpan FixedDrawTimeStep { get; set; }

		public static ILauncher Launcher { get; private set; }

		static LiqueurSystem ()
		{
			CurrentCulture = CultureInfo.CurrentCulture;
			FixedUpdateTimeStep = new TimeSpan ();
			FixedDrawTimeStep = new TimeSpan ();
		}

		public static bool SkipInitializeException { get; set; }

		public static void Run ( ILauncher launcher, Node mainNode, Action initializeEvent = null, params object [] args )
		{
			IWindow window = null; IGraphicsDevice graphicsDevice = null; IAudioDevice audioDevice = null;

			Launcher = launcher;

			try
			{
				launcher.LauncherInitialize ( out window, out graphicsDevice, out audioDevice );
			}
			catch ( Exception e ) { if ( !SkipInitializeException ) throw new Exception ( "Initialization Exception", e ); }
			Window = window;
			GraphicsDevice = graphicsDevice;
			AudioDevice = audioDevice;

			if ( initializeEvent != null )
				initializeEvent ();

			MainNode = mainNode;

			TimeSpan elapsedUpdateTimeStep = new TimeSpan (),
				elapsedDrawTimeStep = new TimeSpan ();
			TimeSpan lastUpdateTimeStep = TimeSpan.FromMilliseconds ( Environment.TickCount ),
				lastDrawTimeStep = TimeSpan.FromMilliseconds ( Environment.TickCount );

			GameTime updateGameTime = new GameTime (), drawGameTime = new GameTime ();

			launcher.LauncherRun ( new LauncherArgument ()
			{
				Initialize = () =>
				{
					if ( mainNode != null )
						mainNode.Intro ( args );
				},
				UpdateLogic = () =>
				{
					if ( elapsedUpdateTimeStep >= FixedUpdateTimeStep || FixedUpdateTimeStep.TotalMilliseconds == 0 )
					{
						updateGameTime.Update ();
						if ( mainNode != null )
							mainNode.Update ( updateGameTime );
						elapsedUpdateTimeStep -= FixedUpdateTimeStep;
					}
					else
					{
						TimeSpan temp = TimeSpan.FromMilliseconds ( Environment.TickCount );
						elapsedUpdateTimeStep += ( temp - lastUpdateTimeStep );
						lastUpdateTimeStep = temp;
					}
				},
				DrawLogic = () =>
				{
					if ( LiqueurSystem.AudioDevice != null )
						LiqueurSystem.AudioDevice.Update ();

					if ( elapsedDrawTimeStep >= FixedDrawTimeStep || FixedDrawTimeStep.TotalMilliseconds == 0 )
					{
						drawGameTime.Update ();
						if ( mainNode != null )
							mainNode.Draw ( drawGameTime );
						LiqueurSystem.GraphicsDevice.SwapBuffer ();
						elapsedDrawTimeStep -= FixedDrawTimeStep;
					}
					else
					{
						TimeSpan temp = TimeSpan.FromMilliseconds ( Environment.TickCount );
						elapsedDrawTimeStep += ( temp - lastDrawTimeStep );
						lastDrawTimeStep = temp;
					}
				},
				Resize = () =>
				{
					if ( mainNode != null && mainNode is IWindowEvent )
						( mainNode as IWindowEvent ).WindowResize ();
				},
				Activated = () =>
				{
					if ( mainNode != null && mainNode is IWindowEvent )
						( mainNode as IWindowEvent ).WindowActivated ();
				},
				Deactivated = () =>
				{
					if ( mainNode != null && mainNode is IWindowEvent )
						( mainNode as IWindowEvent ).WindowDeactivated ();
				}
			} );
		}

		public static void Exit ()
		{
			if ( MainNode != null )
				MainNode.Outro ();
			MainNode = null;
			Launcher.LauncherFinalize ( Window, GraphicsDevice, AudioDevice );
		}
	}
}
