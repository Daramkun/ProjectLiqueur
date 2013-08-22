using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Mathematics.Transforms;

namespace Daramkun.Liqueur.Spirit.Graphics
{
	public sealed class Sprite
	{
		private struct SpriteVertex
		{
			public Vector2 Position;
			public Color Diffuse;
			public Vector2 TexCoord;

			public SpriteVertex ( Vector2 pos, Color dif, Vector2 tex )
			{
				Position = pos;
				Diffuse = dif;
				TexCoord = tex;
			}
		}

		private const FlexibleVertexFormat SpriteVertexFormat = FlexibleVertexFormat.PositionXY |
				FlexibleVertexFormat.Diffuse | FlexibleVertexFormat.TextureUV1;

		static IIndexBuffer indexBuffer;
		static int indexReference;

		IVertexBuffer<SpriteVertex> vertexBuffer;
		Rectangle clippingArea;
		Color overlayColor;

		public IEffect Effect { get; set; }
		public ITexture2D Texture { get; set; }

		public Rectangle ClippingArea
		{
			get { return clippingArea; }
			set
			{
				clippingArea = value;
				SpriteVertex [] vertices = vertexBuffer.Vertices;
				vertices [ 0 ].TexCoord = new Vector2 ( clippingArea.Position.X / Texture.Width, clippingArea.Position.Y / Texture.Height );
				vertices [ 1 ].TexCoord = new Vector2 ( clippingArea.Size.X / Texture.Width, clippingArea.Position.Y / Texture.Height );
				vertices [ 2 ].TexCoord = new Vector2 ( clippingArea.Position.X / Texture.Width, clippingArea.Size.Y / Texture.Height );
				vertices [ 3 ].TexCoord = new Vector2 ( clippingArea.Size.X / Texture.Width, clippingArea.Size.Y / Texture.Height );
				vertexBuffer.Vertices = vertices;
			}
		}

		public Color OverlayColor
		{
			get { return overlayColor; }
			set
			{
				overlayColor = value;
				SpriteVertex [] vertices = vertexBuffer.Vertices;
				vertices [ 0 ].Diffuse = vertices [ 1 ].Diffuse = vertices [ 2 ].Diffuse = vertices [ 3 ].Diffuse = value;
				vertexBuffer.Vertices = vertices;
			}
		}

		public Sprite ( ITexture2D texture )
			: this ( texture, new SpriteEffect () )
		{ }

		public Sprite ( ITexture2D texture, IEffect effect )
		{
			Texture = texture;
			Effect = effect;

			vertexBuffer = LiqueurSystem.GraphicsDevice.CreateVertexBuffer<SpriteVertex> ( SpriteVertexFormat, 4 );
			if ( indexBuffer == null )
				indexBuffer = LiqueurSystem.GraphicsDevice.CreateIndexBuffer ( new int [] { 0, 1, 2, 1, 3, 2 } );
			indexReference++;

			vertexBuffer.Vertices = new SpriteVertex []
			{
				new SpriteVertex ( new Vector2 ( 0, 0 ), Color.White, new Vector2 ( 0, 0 ) ),
				new SpriteVertex ( new Vector2 ( texture.Width, 0 ), Color.White, new Vector2 ( 1, 0 ) ),
				new SpriteVertex ( new Vector2 ( 0, texture.Height ), Color.White, new Vector2 ( 0, 1 ) ),
				new SpriteVertex ( new Vector2 ( texture.Width, texture.Height ), Color.White, new Vector2 ( 1, 1 ) ),
			};

			clippingArea = new Rectangle ( 0, 0, texture.Width, texture.Height );
		}

		public void Dispose ()
		{
			if ( Effect is SpriteEffect )
				Effect.Dispose ();
			Effect = null;

			indexReference--;
			if ( indexReference == 0 )
			{
				indexBuffer.Dispose ();
				indexBuffer = null;
			}
			vertexBuffer.Dispose ();
			vertexBuffer = null;
		}

		public void Draw ( World2 transform )
		{
			Effect.SetTextures ( new TextureArgument () { Texture = Texture, Uniform = "texture0" } );
			Effect.SetArgument<Matrix4x4> ( "projectionMatrix", new OrthographicProjection (
				0, LiqueurSystem.GraphicsDevice.ScreenSize.X, LiqueurSystem.GraphicsDevice.ScreenSize.Y, 0,
				0.0001f, 1000.0f
			).Matrix );
			Effect.SetArgument<Matrix4x4> ( "worldMatrix", transform.Matrix );
			Effect.Dispatch ( ( IEffect effect ) =>
			{
				LiqueurSystem.GraphicsDevice.Draw<SpriteVertex> ( PrimitiveType.TriangleList, vertexBuffer, indexBuffer );
			} );
		}

		public void ResetClippingArea ()
		{
			ClippingArea = new Rectangle ( new Vector2 (), Texture.Size );
		}
	}
}
