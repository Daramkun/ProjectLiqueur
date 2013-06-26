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
using Daramkun.Liqueur.Graphics.Vertices;

namespace Test
{
	struct MyVertex : IFlexibleVertexPositionXY, IFlexibleVertexDiffuse
	{
		public Vector2 Position { get; set; }
		public Color Diffuse { get; set; }
	}

	static class Program
	{
		class MyScene : Scene
		{
			FpsCalculator fpsCalc;

			IPrimitive<MyVertex> primitive;

			Sprite sprite;

			public MyScene ()
			{
			}

			public override void OnInitialize ()
			{
				LiqueurSystem.Window.Title = "Test Window";

				fpsCalc = new FpsCalculator ();
				AddChild ( fpsCalc );
				
				primitive = LiqueurSystem.Renderer.CreatePrimitive<MyVertex> ( 3, 0 );
				primitive.Vertices [ 0 ] = new MyVertex () { Position = new Vector2 ( 200, 100 ), Diffuse = Color.Red };
				primitive.Vertices [ 1 ] = new MyVertex () { Position = new Vector2 ( 100, 300 ), Diffuse = Color.Green };
				primitive.Vertices [ 2 ] = new MyVertex () { Position = new Vector2 ( 300, 300 ), Diffuse = Color.Blue };
				primitive.PrimitiveType = PrimitiveType.TriangleList;
				primitive.PrimitiveCount = 1;
				
				AddChild ( new Sprite ( WalnutSystem.MainContents.Load<ITexture2D> ( "a02.bmp", Color.Magenta ) )
				{
					Position = new Vector2 ( 20, 20 ),
					SourceRectangle = new Rectangle ( new Vector2 ( 20, 20 ), new Vector2 ( 100, 100 ) )
				} );
				AddChild ( new Sprite ( WalnutSystem.MainContents.Load<ITexture2D> ( "a02.bmp", Color.Magenta ) )
				{
					Position = new Vector2 ( 140, 20 ),
					OverlayColor = new Color ( 127, 0, 0, 127 ),
				} );
				sprite = AddChild ( new Sprite ( null )
				{
					Position = new Vector2 ( 340, 100 ),
				} ) as Sprite;

				AddChild ( new Label ( WalnutSystem.MainContents.Load<BaseFont> ( "test.lsf" ) )
				{
					Text = "Test한글도 잘 나옴★ひらかなもキラン☆0123漢字`!@#$%?.,ⓕ\\\n" +
					"LSF 폰트 파일 로드가 좀 느리네\n" + "ZIPLSF 파일 로드 엄청 빨라짐\n" + "뷁뷝뿗颬",
					Position = new Vector2 ( 10, 380 ),
					ForeColor = Color.White
				} );
				AddChild ( new Label ( WalnutSystem.MainContents.Load<BaseFont> ( "test.lsf" ) )
				{
					Position = new Vector2 ( 10, 590 ),
					ForeColor = Color.Cyan,
					ObjectOffset = ObjectOffset.BottomLeft
				} ).Update += ( object sender, GameTimeEventArgs e ) =>
				{
					( sender as Label ).Text = String.Format ( "Update FPS: {0}\nRender FPS: {1}\nBlend State: {2}\nStencil State:{3}\nViewport:{4}", 
						fpsCalc.UpdateFPS, fpsCalc.DrawFPS, LiqueurSystem.Renderer.BlendState, LiqueurSystem.Renderer.StencilState,
						LiqueurSystem.Renderer.Viewport );
				};

				base.OnInitialize ();
			}

			public override void OnFinalize ()
			{
				base.OnFinalize ();
			}

			public override void OnUpdate ( GameTime gameTime )
			{
				sprite.Image = WalnutSystem.MainContents.Load<ITexture2D> ( "square.png" );
				base.OnUpdate ( gameTime );
			}

			public override void OnDraw ( GameTime gameTime )
			{
				LiqueurSystem.Renderer.Clear ( Color.Black );
				LiqueurSystem.Renderer.BlendState = true;
				LiqueurSystem.Renderer.DrawPrimitive<MyVertex> ( primitive );
				base.OnDraw ( gameTime );
			}
		}

		[STAThread]
		static void Main ()
		{
			WalnutSystem.SetupDecoders ();
			WalnutSystem.SetupFixedLogicTimeStep ( TimeSpan.FromTicks ( 166666 ), TimeSpan.FromTicks ( 166666 ) );
			WalnutSystem.SetupInputDevices<Keyboard, Mouse, GamePad, TouchPanel, Accelerometer> ();
			WalnutSystem.Run<Launcher, NoneScriptEngine, MyScene> ( new LocalFileSystem () );
		}
	}
}
