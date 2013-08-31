using System;
using System.Collections.Generic;
using System.Linq;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Platforms;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;

namespace Test.WindowsPhone.CSharp
{
	public class Game1 : Microsoft.Xna.Framework.Game, GraphicsDeviceManagerInGame
	{
		class InternalNode : Node
		{
			public override void Intro ( params object [] args )
			{
				base.Intro ( args );
			}

			public override void Update ( Daramkun.Liqueur.Common.GameTime gameTime )
			{
				base.Update ( gameTime );
			}

			public override void Draw ( Daramkun.Liqueur.Common.GameTime gameTime )
			{
				LiqueurSystem.GraphicsDevice.Clear ( Daramkun.Liqueur.Graphics.ClearBuffer.AllBuffer,
					Daramkun.Liqueur.Graphics.Color.Magenta );

				base.Draw ( gameTime );
			}

			public override void Outro ()
			{
				base.Outro ();
			}
		}

		public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

		public Game1 ()
		{
			GraphicsDeviceManager = new Microsoft.Xna.Framework.GraphicsDeviceManager ( this );
			Content.RootDirectory = "Content";
			InactiveSleepTime = TimeSpan.FromSeconds ( 1 );
		}

		protected override void Initialize ()
		{
			base.Initialize ();
		}

		protected override void LoadContent ()
		{
			LiqueurSystem.Run ( new Launcher ( this ), new InternalNode () );
		}

		protected override void UnloadContent ()
		{
			LiqueurSystem.Exit ();
		}

		protected override void Update ( GameTime gameTime )
		{
			if ( GamePad.GetState ( PlayerIndex.One ).Buttons.Back == ButtonState.Pressed )
				this.Exit ();
		}

		bool beginDraw = true;
		protected override bool BeginDraw ()
		{
			return beginDraw = !beginDraw;
		}
	}
}
