using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.Decoder.Audios;
using Daramkun.Liqueur.Contents.Decoder.Images;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Mathematics.Transforms;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Platforms;

namespace Test.Windows.CSharp
{
	static class Program
	{
		public struct Vertex
		{
			Vector2 position;
			Vector2 texture;

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

			public override void Intro ( params object [] args )
			{
				//LiqueurSystem.GraphicsDevice.FullscreenMode = true;
				LiqueurSystem.GraphicsDevice.ScreenSize = new Vector2 ( 1024, 768 );
				//( LiqueurSystem.Window as IDesktopWindow ).IsResizable = true;
				LiqueurSystem.GraphicsDevice.CullingMode = CullingMode.CounterClockWise;
				LiqueurSystem.GraphicsDevice.Viewport = new Viewport () { X = 0, Y = 0, Width = 1024, Height = 768 };

				vertexShader = LiqueurSystem.GraphicsDevice.CreateShader ( @"#version 150
layout(location = 0) in vec3 a_position;
layout(location = 1) in vec2 a_texcoord;

uniform mat4 proj;
uniform mat4 modelView;

out vec2 v_texcoord;

void main () {
	vec4 pos = vec4(a_position, 1);
	pos = modelView * pos;
	pos = proj * pos;
	gl_Position = pos;
	v_texcoord = a_texcoord;
}
					", ShaderType.VertexShader );
				fragShader = LiqueurSystem.GraphicsDevice.CreateShader ( @"#version 150
in vec2 v_texcoord;

uniform sampler2D texture;

void main () {
	gl_FragColor = texture2D ( texture, v_texcoord.st );
}
					", ShaderType.FragmentShader );
				effect = LiqueurSystem.GraphicsDevice.CreateEffect ( vertexShader, fragShader );
				effect.SetArgument<Matrix4x4> ( "modelView", Matrix4x4.Identity * new View ( new Vector3 ( -7, 7, 10 ), 
					new Vector3 ( 0f, 0f, 0f ), 
					new Vector3 ( 0, 1, 0 ) ).Matrix );
				effect.SetArgument<Matrix4x4> ( "proj", new PerspectiveFieldOfViewProjection ( ( float ) Math.PI / 4, 800 / 600.0f, 0.0001f, 1000.0f ).Matrix );

				Matrix4x4 test = effect.GetArgument<Matrix4x4> ( "proj" );

				vertexBuffer = LiqueurSystem.GraphicsDevice.CreateVertexBuffer<Vertex> ( FlexibleVertexFormat.PositionXY |
					FlexibleVertexFormat.Diffuse, new Vertex []
				{
					new Vertex ( new Vector2 ( -5.0f, +5.0f ), new Vector2 ( 0, 1 ) ),
					new Vertex ( new Vector2 ( +5.0f, -5.0f ), new Vector2 ( 1, 0 ) ),
					new Vertex ( new Vector2 ( -5.0f, -5.0f ), new Vector2 ( 0, 0 ) ),
					new Vertex ( new Vector2 ( +5.0f, +5.0f ), new Vector2 ( 1, 1 ) ),
				} );
				indexBuffer = LiqueurSystem.GraphicsDevice.CreateIndexBuffer ( new int [] { 0, 2, 1, 0, 1, 3, } );

				using ( FileStream fs = new FileStream ( "temp.png", FileMode.Open ) )
					texture = LiqueurSystem.GraphicsDevice.CreateTexture2D ( new PngDecoder ().Decode ( fs ) );

				renderBuffer = LiqueurSystem.GraphicsDevice.CreateRenderBuffer ( 800, 600 );

				LiqueurSystem.GraphicsDevice.BlendState = true;
				LiqueurSystem.GraphicsDevice.BlendOperation = BlendOperation.AlphaBlend;

				base.Intro ( args );
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
				base.Draw ( gameTime );
			}

			public override void Outro ()
			{
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

		[STAThread]
		static void Main ()
		{
			LiqueurSystem.Run ( new Launcher (), new InternalScene () );
		}
	}
}
