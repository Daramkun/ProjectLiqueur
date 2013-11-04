using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Spirit.Nodes;

namespace Test.Game.Dodge.Controllers
{
	public class PlayerController : Node
	{
		public override void Intro ( params object [] args )
		{
			SpriteNode chr = new SpriteNode ( ( Parent as GameScene ).Contents.Load<ITexture2D> ( "Test.Game.Dodge.Resources.daram.bmp", Color.Magenta ) );
			chr.Alignment = SpriteAlignment.CenterMiddle;
			chr.World.Translate = LiqueurSystem.GraphicsDevice.ScreenSize / 2;
			Add ( chr );
			base.Intro ( args );
		}

		public override void Update ( GameTime gameTime )
		{
			if ( InputHelper.CurrentKeyboardState.IsKeyDown ( KeyboardKey.Up ) )
				( this [ 0 ] as SpriteNode ).World.Translate += new Vector2 ( 0, -1 ) * ( gameTime.ElapsedGameTime.Milliseconds / 5.0f );
			if ( InputHelper.CurrentKeyboardState.IsKeyDown ( KeyboardKey.Down ) )
				( this [ 0 ] as SpriteNode ).World.Translate += new Vector2 ( 0, 1 ) * ( gameTime.ElapsedGameTime.Milliseconds / 5.0f );
			if ( InputHelper.CurrentKeyboardState.IsKeyDown ( KeyboardKey.Left ) )
				( this [ 0 ] as SpriteNode ).World.Translate += new Vector2 ( -1, 0 ) * ( gameTime.ElapsedGameTime.Milliseconds / 5.0f );
			if ( InputHelper.CurrentKeyboardState.IsKeyDown ( KeyboardKey.Right ) )
				( this [ 0 ] as SpriteNode ).World.Translate += new Vector2 ( 1, 0 ) * ( gameTime.ElapsedGameTime.Milliseconds / 5.0f );
			base.Update ( gameTime );
		}
	}
}
