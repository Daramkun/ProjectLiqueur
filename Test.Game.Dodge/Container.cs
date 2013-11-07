using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Nodes.Scenes;
using Daramkun.Liqueur.Spirit.Graphics;

namespace Test.Game.Dodge
{
	public class Container : Node
	{
		ContentManager contentManager;
		Font fpsFont;

		public SceneContainer SceneContainer { get; set; }

		public Container ()
		{
			Texture2DContentLoader.AddDefaultDecoders ();
			ContentManager.ContentLoaderAssemblies.Add ( Assembly.Load ( "Daramkun.Liqueur.Spirit" ) );
		}

		public override void Intro ( params object [] args )
		{
			LiqueurSystem.Window.Title = "Simple Dodge";
			LiqueurSystem.GraphicsDevice.BlendState = true;
			LiqueurSystem.GraphicsDevice.BlendOperation = BlendOperation.AlphaBlend;

			Add ( InputHelper.CreateInstance () );
			InputHelper.IsKeyboardEnabled = true;
			Add ( SceneContainer = new SceneContainer ( new MenuScene () ) );
			
			contentManager = new ContentManager ( FileSystemManager.GetFileSystem ( "ManifestFileSystem" ) );
			contentManager.AddDefaultContentLoader ();
			LiqueurSystem.Launcher.InvokeInMainThread ( () =>
			{
				fpsFont = contentManager.Load<TrueTypeFont> ( "Test.Game.Dodge.Resources.GameFont.ttf", 20 );
			} );

			FpsCalculator calc;
			Add ( calc = new FpsCalculator () );
			calc.DrawEvent += ( object sender, GameTimeEventArgs e ) =>
			{
				string fpsString = string.Format ( "Update FPS:{0:0.00}\nRender FPS:{1:0.00}", calc.UpdateFPS, calc.DrawFPS );
				fpsFont.DrawFont ( fpsString, Color.White, 
					LiqueurSystem.GraphicsDevice.ScreenSize - fpsFont.MeasureString ( fpsString ) - new Vector2 ( 10, 10 ) );
			};

			base.Intro ( args );
		}

		public override void Outro ()
		{
			contentManager.Dispose ();
			base.Outro ();
		}

		public override void Update ( GameTime gameTime )
		{
			base.Update ( gameTime );
		}

		public override void Draw ( GameTime gameTime )
		{
			LiqueurSystem.GraphicsDevice.Clear ( ClearBuffer.AllBuffer, Color.Black );
			base.Draw ( gameTime );
		}
	}
}
