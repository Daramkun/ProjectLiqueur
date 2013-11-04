using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Nodes.Scenes;
using Daramkun.Liqueur.Spirit.Graphics;
using Test.Game.Dodge.Controllers;

namespace Test.Game.Dodge
{
	public class GameScene : Node
	{
		public ContentManager Contents { get; private set; }

		bool isGameOver;
		public bool IsGameOver
		{
			get { return isGameOver; }
			set
			{
				isGameOver = value;
				this [ 0 ].IsEnabled = this [ 1 ].IsEnabled = !value;
			}
		}

		Font gameOverFont, timeStampFont;
		TimeSpan timeStamp;

		public override void Intro ( params object [] args )
		{
			Contents = new ContentManager ( new ManifestFileSystem () );
			Contents.AddDefaultContentLoader ();

			Add ( new PlayerController () );
			Add ( new BulletController () );

			gameOverFont = Contents.Load<TrueTypeFont> ( "Test.Game.Dodge.Resources.GameFont.ttf", 64 );
			timeStampFont = Contents.Load<TrueTypeFont> ( "Test.Game.Dodge.Resources.GameFont.ttf", 32 );
			base.Intro ( args );
		}

		public override void Outro ()
		{
			base.Outro ();
		}

		public override void Update ( GameTime gameTime )
		{
			if ( !isGameOver )
			{
				timeStamp += gameTime.ElapsedGameTime;
			}
			else
			{
				if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.Space ) )
				{
					( Parent as SceneContainer ).Transition ( new MenuScene () );
				}
			}
			base.Update ( gameTime );
		}

		public override void Draw ( GameTime gameTime )
		{
			base.Draw ( gameTime );

			string currentTime = string.Format ( "ELAPSED: {0:0.00}sec", timeStamp.TotalSeconds );
			timeStampFont.DrawFont ( currentTime, Color.White,
				new Vector2 () );

			if ( isGameOver )
			{
				gameOverFont.DrawFont ( "GAME OVER", Color.White,
					LiqueurSystem.GraphicsDevice.ScreenSize / 2 - gameOverFont.MeasureString ( "GAME OVER" ) / 2 );
				string noticeString = "PRESS SPACEBAR TO MENU";
				timeStampFont.DrawFont ( noticeString, Color.White, ( LiqueurSystem.GraphicsDevice.ScreenSize / 2 -
					gameOverFont.MeasureString ( noticeString ) / 2 ) + new Vector2 ( 0, 48 ) );
			}
		}
	}
}
