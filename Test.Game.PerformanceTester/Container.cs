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
		Node [] nodes;
		ITexture2D [] textures;

		ContentManager contentManager;

		public override void Intro ( params object [] args )
		{
			LiqueurSystem.GraphicsDevice.CullingMode = CullingMode.None;

			Add ( InputHelper.CreateInstance () );
			
			FpsCalculator calc = new FpsCalculator ();
			calc.DrawEvent += ( object sender, GameTimeEventArgs e ) =>
			{
				int childrenCount = 0;
				for ( int i = 0; i < 6; ++i )
					childrenCount += nodes [ i ].ChildrenCount;
				LiqueurSystem.Window.Title = string.Format ( "Update FPS: {0}, Draw FPS: {1}, Children count: {2}",
					calc.UpdateFPS, calc.DrawFPS, childrenCount );
			};
			Add ( calc );

			ContentManager.ContentLoaderAssemblies.Add ( Assembly.Load ( "Daramkun.Liqueur.Spirit" ) );
			contentManager = new ContentManager ( FileSystemManager.GetFileSystem ( "ManifestFileSystem" ) );
			contentManager.AddDefaultContentLoader ();
			Texture2DContentLoader.AddDefaultDecoders ();

			textures = new ITexture2D [ 6 ];
			textures [ 0 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.0096x0096.png" );
			textures [ 1 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.0128x0128.png" );
			textures [ 2 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.0256x0256.png" );
			textures [ 3 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.0512x0512.png" );
			textures [ 4 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.1024x1024.png" );
			textures [ 5 ] = contentManager.Load<ITexture2D> ( "Test.Game.PerformanceTester.Resources.2048x2048.png" );

			nodes = new Node [ 6 ];
			for ( int i = 0; i < 6; ++i )
			{
				nodes [ i ] = Add ( new Node () );
			}

			base.Intro ( args );
		}

		public override void Outro ()
		{
			contentManager.Dispose ();
			base.Outro ();
		}

		private void Add ( int mode, int count )
		{
			nodes [ mode ].IsManuallyChildrenCacheMode = true;
			LiqueurSystem.Launcher.InvokeInMainThread ( () =>
			{
				for ( int i = 0; i < count; ++i )
				{
					nodes [ mode ].Add ( new PerformanceSpriteNode ( textures [ mode ] ) );
				}
				nodes [ mode ].RefreshChildrenCache ();
			}, false );
		}

		private void Remove ( int mode, int count )
		{
			nodes [ mode ].IsManuallyChildrenCacheMode = true;
			for ( int i = 0; i < count; ++i )
			{
				if ( nodes [ mode ].ChildrenCount <= 0 ) continue;
				nodes [ mode ].Remove ( nodes [ mode ] [ nodes [ mode ].ChildrenCount - 1 ] );
			}
			nodes [ mode ].RefreshChildrenCache ();
		}

		public override void Update ( GameTime gameTime )
		{
			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.Q ) )
				Add ( 0, 100 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.W ) )
				Add ( 1, 100 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.E ) )
				Add ( 2, 100 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.R ) )
				Add ( 3, 100 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.T ) )
				Add ( 4, 100 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.Y ) )
				Add ( 5, 100 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.A ) )
				Remove ( 0, 100 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.S ) )
				Remove ( 1, 100 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.D ) )
				Remove ( 2, 100 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.F ) )
				Remove ( 3, 100 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.G ) )
				Remove ( 4, 100 );

			if ( InputHelper.IsKeyboardKeyUpRightNow ( KeyboardKey.H ) )
				Remove ( 5, 100 );

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
