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
			contentManager = new ContentManager ( FileSystemManager.GetFileSystem ( "ManifestFileSystem" ) );
			contentManager.AddDefaultContentLoader ();
			Texture2DContentLoader.AddDefaultDecoders ();

			textures = new ITexture2D [ 4 ];
			textures [ 0 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.test1.png" );
			textures [ 1 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.test2.png" );
			textures [ 2 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.test3.png" );
			textures [ 3 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.test4.png" );

			Add ( 100 );

			base.Intro ( args );
		}

		public override void Outro ()
		{
			contentManager.Dispose ();
			base.Outro ();
		}

		private void Add (int count)
		{
			IsManuallyChildrenCacheMode = true;
			LiqueurSystem.Launcher.InvokeInMainThread ( () =>
			{
				for ( int i = 0; i < count; ++i )
				{
					Add ( new PerformanceSpriteNode ( textures [ random.Next ( 4 ) ] ) );
				}
				RefreshChildrenCache ();
			}, false );
		}

		private void Remove (int count)
		{
			IsManuallyChildrenCacheMode = true;
			for ( int i = 0; i < count; ++i )
			{
				Remove ( this [ ChildrenCount - 1 ] );
			}
			RefreshChildrenCache ();
		}

		public override void Update ( GameTime gameTime )
		{
			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.Q ) )
				Add ( 10 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.W ) )
				Remove ( 10 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.A ) )
				Add ( 100 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.S ) )
				Remove ( 100 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.Z ) )
				Add ( 1000 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.X ) )
				Remove ( 1000 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.Space ) )
				Add ( 10000 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.Return ) )
				Remove ( 10000 );

			base.Update ( gameTime );
		}

		public override void Draw ( GameTime gameTime )
		{
			LiqueurSystem.GraphicsDevice.Clear ( ClearBuffer.AllBuffer, Color.Black );
			LiqueurSystem.GraphicsDevice.BeginScene ();
			base.Draw ( gameTime );
			LiqueurSystem.GraphicsDevice.EndScene ();
		}
    }
}
