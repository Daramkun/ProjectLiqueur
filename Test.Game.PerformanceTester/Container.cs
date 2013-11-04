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
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Platforms;

namespace Test.Game.PerformanceTester
{
    public class Container : Node
    {
		Random random = new Random ( Environment.TickCount );
		ITexture2D [] textures;

		ContentManager contentManager;

		public override void Intro ( params object [] args )
		{
			Add ( InputHelper.CreateInstance () );
			
			FpsCalculator calc = new FpsCalculator ();
			calc.DrawEvent += ( object sender, GameTimeEventArgs e ) =>
			{
				LiqueurSystem.Window.Title = string.Format ( "Update FPS: {0}, Draw FPS: {1}, Children count: {2}",
					calc.UpdateFPS, calc.DrawFPS, ChildrenCount );
			};
			Add ( calc );

			ContentManager.ContentLoaderAssemblies.Add ( Assembly.Load ( "Daramkun.Liqueur.Spirit" ) );
			contentManager = new ContentManager ( new ManifestFileSystem () );
			contentManager.AddDefaultContentLoader ();
			Texture2DContentLoader.AddDefaultDecoders ();

			textures = new ITexture2D [ 4 ];
			textures [ 0 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.test1.png" );
			textures [ 1 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.test2.png" );
			textures [ 2 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.test3.png" );
			textures [ 3 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.test4.png" );

			base.Intro ( args );
		}

		public override void Outro ()
		{
			contentManager.Dispose ();
			base.Outro ();
		}

		private void Add ()
		{
			LiqueurSystem.Launcher.InvokeInMainThread ( () =>
			{
				Add ( new PerformanceSpriteNode ( textures [ random.Next ( 4 ) ] ) );
			}, false );
		}

		private void Remove ()
		{
			Remove ( this [ ChildrenCount - 1 ] );
		}

		public override void Update ( GameTime gameTime )
		{
			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.Q ) )
				for ( int i = 0; i < 10; i++ )
					Add ();

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.W ) )
				for ( int i = 0; i < 10; i++ )
					Remove ();

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.A ) )
				for ( int i = 0; i < 100; i++ )
					Add ();

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.S ) )
				for ( int i = 0; i < 100; i++ )
					Remove ();

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.Z ) )
				for ( int i = 0; i < 1000; i++ )
					Add ();

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.X ) )
				for ( int i = 0; i < 1000; i++ )
					Remove ();

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.Space ) )
				for ( int i = 0; i < 10000; i++ )
					Add ();

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.Return ) )
				for ( int i = 0; i < 10000; i++ )
					Remove ();

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.T ) )
			{
				/*if ( UpdateLooper == null )
					UpdateLooper = ForEachCollection.GetForEach ( "Parallel" );
				else
					UpdateLooper = null;*/
			}

			base.Update ( gameTime );
		}

		public override void Draw ( GameTime gameTime )
		{
			LiqueurSystem.GraphicsDevice.Clear ( ClearBuffer.AllBuffer, Color.Black );
			base.Draw ( gameTime );
		}
    }
}
