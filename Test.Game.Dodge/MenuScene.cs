using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Nodes.Scenes;
using Daramkun.Liqueur.Spirit.Graphics;

namespace Test.Game.Dodge
{
    public class MenuScene : Node
    {
		ContentManager contentManager;
		Font titleFont;
		Font menuFont;

		public override void Intro ( params object [] args )
		{
			contentManager = new ContentManager ( FileSystemManager.GetFileSystem ( "ManifestFileSystem" ) );
			contentManager.AddDefaultContentLoader ();
			LiqueurSystem.Launcher.InvokeInMainThread ( () =>
			{
				titleFont = contentManager.Load<TrueTypeFont> ( "Test.Game.Dodge.Resources.GameFont.ttf", 64 );
				menuFont = contentManager.Load<TrueTypeFont> ( "Test.Game.Dodge.Resources.GameFont.ttf", 24 );
			} );
			base.Intro ( args );
		}

		public override void Outro ()
		{
			contentManager.Dispose ();
			base.Outro ();
		}

		public override void Update ( GameTime gameTime )
		{
			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.A ) )
			{
				( Parent as SceneContainer ).Transition ( new GameScene () );
			}
			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.B ) )
			{
				LiqueurSystem.Launcher.InvokeInMainThread ( () =>
				{
					LiqueurSystem.Exit ();
				} );
			}
			base.Update ( gameTime );
		}

		public override void Draw ( GameTime gameTime )
		{
			titleFont.DrawFont ( "Dodge", Color.White, new Vector2 ( 24 ) );

			menuFont.DrawFont ( "A. START", Color.White, new Vector2 ( 24, 256 ) );
			menuFont.DrawFont ( "B. EXIT", Color.White, new Vector2 ( 24, 304 ) );

			base.Draw ( gameTime );
		}
    }
}
