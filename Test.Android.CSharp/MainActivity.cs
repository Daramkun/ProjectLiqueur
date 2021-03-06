using System;
using System.Reflection;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Spirit.Graphics;
using Daramkun.Liqueur.Contents.Decoder.Images;
using Daramkun.Liqueur.Mathematics.Transforms;

namespace Test.Android.CSharp
{
	[Activity (Label = "Test.Android.CSharp", MainLauncher = true)]
	public class MainActivity : Activity
	{
		public struct Vertex
		{
			public Vector2 position;
			public Vector2 texture;

			public Vertex ( Vector2 position, Vector2 texture )
			{
				this.position = position;
				this.texture = texture;
			}
		}

		class InternalScene : Node
		{
			IShader vertexShader, fragShader;
			IEffect effect;
			IVertexBuffer<Vertex> vertexBuffer;
			IIndexBuffer indexBuffer;
			ITexture2D texture;
			IRenderBuffer renderBuffer;
			Sprite sprite;
			float angle;
			LsfFont font;

			public override void Intro ( params object [] args )
			{
				LiqueurSystem.GraphicsDevice.IsZWriteEnable = true;
				LiqueurSystem.GraphicsDevice.CullingMode = CullingMode.None;
				LiqueurSystem.GraphicsDevice.ScreenSize = new Vector2 ( 800, 600 );
				LiqueurSystem.GraphicsDevice.BlendState = true;
				LiqueurSystem.GraphicsDevice.BlendOperation = BlendOperation.AlphaBlend;

				vertexShader = LiqueurSystem.GraphicsDevice.CreateShader ( @"#version 120
attribute vec3 a_position;
attribute vec2 a_texcoord;

uniform mat4 proj;
uniform mat4 modelView;

varying vec2 v_texcoord;

void main () {
	vec4 pos = vec4(a_position, 1);
	pos = modelView * pos;
	pos = proj * pos;
	gl_Position = pos;
	v_texcoord = a_texcoord;
}
					", ShaderType.VertexShader );
				vertexShader.Option = new ShaderOption ()
				{
					AttributeOrdering = new ShaderOption.AttributeOrder []
					{
						new ShaderOption.AttributeOrder () { Name = "a_position", VertexType = FlexibleVertexFormat.PositionXYZ },
						new ShaderOption.AttributeOrder () { Name = "a_texcoord", VertexType = FlexibleVertexFormat.TextureUV1 }
					}
				};
				fragShader = LiqueurSystem.GraphicsDevice.CreateShader ( @"#version 120
varying vec2 v_texcoord;

uniform sampler2D texture;

void main () {
	gl_FragColor = texture2D ( texture, v_texcoord.st );
}
					", ShaderType.FragmentShader );
				effect = LiqueurSystem.GraphicsDevice.CreateEffect ( vertexShader, fragShader );
				effect.SetArgument<Matrix4x4> ( "modelView", Matrix4x4.Identity * new Daramkun.Liqueur.Mathematics.Transforms.View ( new Vector3 ( -7, 7, 10 ), 
				                                                                            new Vector3 ( 0f, 0f, 0f ), 
				                                                                            new Vector3 ( 0, 1, 0 ) ).Matrix );
				effect.SetArgument<Matrix4x4> ( "proj", new PerspectiveFieldOfViewProjection ( ( float ) Math.PI / 4, 800 / 600.0f, 0.0001f, 1000.0f ).Matrix );

				vertexBuffer = LiqueurSystem.GraphicsDevice.CreateVertexBuffer<Vertex> ( FlexibleVertexFormat.PositionXY |
				                                                                        FlexibleVertexFormat.Diffuse, new Vertex []
				                                                                        {
					new Vertex ( new Vector2 ( -5.0f, +5.0f ), new Vector2 ( 0, 1 ) ),
					new Vertex ( new Vector2 ( +5.0f, -5.0f ), new Vector2 ( 1, 0 ) ),
					new Vertex ( new Vector2 ( -5.0f, -5.0f ), new Vector2 ( 0, 0 ) ),
					new Vertex ( new Vector2 ( +5.0f, +5.0f ), new Vector2 ( 1, 1 ) ),
				} );
				indexBuffer = LiqueurSystem.GraphicsDevice.CreateIndexBuffer ( new int [] { 0, 2, 1, 0, 1, 3, } );

				texture = LiqueurSystem.GraphicsDevice.CreateTexture2D ( new PngDecoder ().Decode (
					Assembly.GetEntryAssembly ().GetManifestResourceStream ( "Test.Windows.CSharp.temp.png" ) )
				                                                        );

				renderBuffer = LiqueurSystem.GraphicsDevice.CreateRenderBuffer ( 1024, 1024 );

				sprite = new Sprite ( texture );
				font = new LsfFont ( Assembly.GetEntryAssembly ().GetManifestResourceStream ( "Test.Windows.CSharp.temp.lsf" ) );

				FpsCalculator calc = new FpsCalculator ();
				calc.DrawEvent += ( object sender, GameTimeEventArgs e ) =>
				{
					string fpsString = string.Format ( "Update FPS: {0:0.00}\nRender FPS: {1:0.00}", calc.UpdateFPS, calc.DrawFPS );
					font.DrawFont ( fpsString, Color.Black,
					               new Vector2 ( 0, LiqueurSystem.GraphicsDevice.ScreenSize.Y - font.MeasureString ( fpsString ).Y ) );
				};
				Add ( calc );

				base.Intro ( args );
			}

			public override void Update ( GameTime gameTime )
			{
				angle += gameTime.ElapsedGameTime.Milliseconds / 20000.0f;
				base.Update ( gameTime );
			}

			public override void Draw ( GameTime gameTime )
			{
				LiqueurSystem.GraphicsDevice.RenderTarget = renderBuffer;
				LiqueurSystem.GraphicsDevice.Clear ( ClearBuffer.AllBuffer, Color.White );
				effect.SetTextures ( new TextureArgument () { Uniform = "texture", Texture = texture } );
				effect.Dispatch ( ( IEffect ef ) =>
				                 {
					LiqueurSystem.GraphicsDevice.Draw<Vertex> ( PrimitiveType.TriangleList, vertexBuffer, indexBuffer );
				} );

				LiqueurSystem.GraphicsDevice.RenderTarget = null;
				LiqueurSystem.GraphicsDevice.Clear ( ClearBuffer.AllBuffer, new Color ( 0.2f, 0.5f, 0.4f, 1.0f ) );

				effect.SetTextures ( new TextureArgument () { Uniform = "texture", Texture = renderBuffer } );
				effect.Dispatch ( ( IEffect ef ) =>
				                 {
					LiqueurSystem.GraphicsDevice.Draw<Vertex> ( PrimitiveType.TriangleList, vertexBuffer, indexBuffer );
				} );

				World2 world = new World2 ( LiqueurSystem.GraphicsDevice.ScreenSize / 2 - sprite.Texture.Size / 2,
				                           new Vector2 ( 1 + angle ), sprite.Texture.Size / 2, angle, sprite.Texture.Size / 2 );
				sprite.Draw ( world );

				font.DrawFont ( "Test (문자열 출력 테스트!) 日本語テスト ♣♪", Color.White, new Vector2 ( 0, 0 ) );

				base.Draw ( gameTime );
			}

			public override void Outro ()
			{
				sprite.Dispose ();
				renderBuffer.Dispose ();
				texture.Dispose ();
				indexBuffer.Dispose ();
				vertexBuffer.Dispose ();
				effect.Dispose ();
				fragShader.Dispose ();
				vertexShader.Dispose ();
				base.Outro ();
			}
		}

		protected override void OnCreate ( Bundle bundle )
		{
			base.OnCreate ( bundle );
			LiqueurSystem.SkipInitializeException = true;
			LiqueurSystem.Run ( new Launcher ( this ), new InternalScene () );
		}
	}
}


