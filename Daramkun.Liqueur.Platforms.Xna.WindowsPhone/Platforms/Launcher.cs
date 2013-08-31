using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Nodes;
using Microsoft.Xna.Framework;

namespace Daramkun.Liqueur.Platforms
{
	public interface GraphicsDeviceManagerInGame
	{
		GraphicsDeviceManager GraphicsDeviceManager { get; }
	}

	public class Launcher : ILauncher
	{
		public PlatformInformation PlatformInformation
		{
			get
			{
				return new PlatformInformation ()
				{
					PlatformType = PlatformType.WindowsPhone,
					PlatformVersion = Environment.OSVersion.Version,
				};
			}
		}

		public bool IsInitialized { get; private set; }

		private Game game;

		public Launcher ( Game game )
		{
			this.game = game;
		}

		public void LauncherInitialize ( out IWindow window, out IGraphicsDevice graphicsDevice, out IAudioDevice audioDevice )
		{
			window = new Window ( game );
			graphicsDevice = new GraphicsDevice ( game );
			audioDevice = null;

			IsInitialized = true;
		}

		private class LiqueurComponent : DrawableGameComponent
		{
			LauncherArgument Argument { get; set; }

			public LiqueurComponent ( Game game, LauncherArgument args ) : base ( game ) { Argument = args; Enabled = true; Visible = true; }

			protected override void LoadContent ()
			{
				Argument.Initialize ();
				base.LoadContent ();
			}

			protected override void Dispose ( bool disposing )
			{
				base.Dispose ( disposing );
			}

			public override void Update ( GameTime gameTime )
			{
				Argument.UpdateLogic ();
				//base.Update ( gameTime );
			}

			public override void Draw ( GameTime gameTime )
			{
				Argument.DrawLogic ();
				//base.Draw ( gameTime );
			}
		}

		public void LauncherRun ( LauncherArgument args )
		{
			game.Window.ClientSizeChanged += ( object sender, EventArgs e ) => { args.Resize (); };
			game.Components.Add ( new LiqueurComponent ( game, args ) );
		}

		public void LauncherFinalize ( IWindow window, IGraphicsDevice graphicsDevice, IAudioDevice audioDevice )
		{
			if ( audioDevice != null ) audioDevice.Dispose ();
			graphicsDevice.Dispose ();
			window.Dispose ();
		}
	}
}
