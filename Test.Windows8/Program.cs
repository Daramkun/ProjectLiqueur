using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Decoders;
using Daramkun.Liqueur.Decoders.Images;
using Daramkun.Liqueur.Decoders.Sounds;
using Daramkun.Liqueur.Math;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Graphics.Fonts;
using Daramkun.Liqueur.Inputs;
using Daramkun.Liqueur.Inputs.Devices;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Platforms;
using Daramkun.Liqueur.Scenes;
using Daramkun.Walnut;
using Daramkun.Walnut.Nodes;
using Daramkun.Walnut.Scripts;

namespace Test.Windows8
{
	static class Program
	{
		class MyScene : Scene
		{
			FpsCalculator fpsCalc;

			public MyScene ()
			{
			}

			public override void OnInitialize ()
			{
				LiqueurSystem.Window.Title = "Test Window";
				
				fpsCalc = new FpsCalculator ();
				AddChild ( fpsCalc );

				AddChild ( new Sprite ( WalnutSystem.MainContents.Load<IImage> ( "Test.Windows8.Assets.goodbye.png", Color.Magenta ) )
				{
					Position = new Vector2 ( 20, 20 ),
					SourceRectangle = new Rectangle ( new Vector2 ( 20, 20 ), new Vector2 ( 200, 200 ) )
				} );
				AddChild ( new Label ( WalnutSystem.MainContents.Load<BaseFont> ( "Test.Windows8.Assets.test.lsf" ) )
				{
					Position = new Vector2 ( 10, 590 ),
					ForeColor = Color.Cyan,
					ObjectOffset = ObjectOffset.BottomLeft
				} ).Update += ( object sender, GameTimeEventArgs e ) =>
				{
					( sender as Label ).Text = String.Format ( "Update FPS: {0}\nRender FPS: {1}", fpsCalc.UpdateFPS, fpsCalc.DrawFPS );
				};

				base.OnInitialize ();
			}

			public override void OnFinalize ()
			{
				base.OnFinalize ();
			}

			public override void OnUpdate ( GameTime gameTime )
			{
				base.OnUpdate ( gameTime );
			}

			public override void OnDraw ( GameTime gameTime )
			{
				LiqueurSystem.Renderer.Clear ( Color.Black );

				base.OnDraw ( gameTime );
			}
		}

		[MTAThread]
		static void Main ()
		{
			WalnutSystem.SetupDecoders ();
			WalnutSystem.SetupFixedLogicTimeStep ( TimeSpan.FromTicks ( 166666 ), TimeSpan.FromTicks ( 166666 ) );
			WalnutSystem.SetupInputDevices<Keyboard, Mouse, GamePad, TouchPanel, Accelerometer> ();
			WalnutSystem.Run<Launcher, NoneScriptEngine, MyScene> ( new ManifestFileSystem () );
		}
	}
}
