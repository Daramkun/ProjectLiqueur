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
			SpriteNode collaps = new SpriteNode ( ( Parent as GameScene ).Contents.Load<ITexture2D> ( "Test.Game.Dodge.Resources.target.bmp", Color.Magenta ) );
			collaps.Alignment = SpriteAlignment.CenterMiddle;
			collaps.World.Translate = LiqueurSystem.GraphicsDevice.ScreenSize / 2;
			collaps.OverlayColor = new Color ( 255, 255, 255, 120 );
			Add ( collaps );
			base.Intro ( args );
		}

		public override void Update ( GameTime gameTime )
		{
			Vector2 pos = ( this [ 0 ] as SpriteNode ).World.Translate;
			if ( InputHelper.CurrentKeyboardState.IsKeyDown ( KeyboardKey.Up ) )
				pos += new Vector2 ( 0, -1 ) * ( gameTime.ElapsedGameTime.Milliseconds / 5.0f );
			if ( InputHelper.CurrentKeyboardState.IsKeyDown ( KeyboardKey.Down ) )
				pos += new Vector2 ( 0, 1 ) * ( gameTime.ElapsedGameTime.Milliseconds / 5.0f );
			if ( InputHelper.CurrentKeyboardState.IsKeyDown ( KeyboardKey.Left ) )
				pos += new Vector2 ( -1, 0 ) * ( gameTime.ElapsedGameTime.Milliseconds / 5.0f );
			if ( InputHelper.CurrentKeyboardState.IsKeyDown ( KeyboardKey.Right ) )
				pos += new Vector2 ( 1, 0 ) * ( gameTime.ElapsedGameTime.Milliseconds / 5.0f );
			( this [ 0 ] as SpriteNode ).World.Translate = ( this [ 1 ] as SpriteNode ).World.Translate = pos;
			base.Update ( gameTime );
		}
	}
}
