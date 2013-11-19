using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.Decoder.Images;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Contents.Loaders;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Mathematics.Transforms;
using Daramkun.Liqueur.Nodes;
using Daramkun.Liqueur.Spirit.Graphics;

namespace Test.Game.Terrain
{
	public struct TerrainVertex
	{
		public Vector3 Position;
		public Color Diffuse;
		public Vector2 Texture;

		public TerrainVertex ( float x, float y, float z, float u, float v )
		{
			Position = new Vector3 ( x, y, z );
			Diffuse = Color.White;
			Texture = new Vector2 ( u, v );
		}
	}

    public class Container : Node
    {
		ContentManager contentManager;

		IVertexBuffer<TerrainVertex> terrainVertex;
		IIndexBuffer terrainIndex;
		ITexture2D texture;
		IEffect terrainEffect;

		public Container ()
		{
			Texture2DContentLoader.AddDefaultDecoders ();
		}

		public override void Intro ( params object [] args )
		{
			IFileSystem fileSystem = FileSystemManager.GetFileSystem ( "ManifestFileSystem" );
			contentManager = new ContentManager ( fileSystem );
			contentManager.AddDefaultContentLoader ();

			ImageInfo imageInfo = new PngDecoder ().Decode ( fileSystem.OpenFile ( "Test.Game.Terrain.Resources.terrain_01.png" ) );
			Color [] decoded = imageInfo.GetPixel ( null );
			
			terrainVertex = LiqueurSystem.GraphicsDevice.CreateVertexBuffer<TerrainVertex> ( FlexibleVertexFormat.PositionXYZ | FlexibleVertexFormat.TextureUV1,
				imageInfo.Width * imageInfo.Height );
			TerrainVertex [] tempVertices = new TerrainVertex [ terrainVertex.Length ];
			int index = 0;
			for ( int x = 0; x < imageInfo.Height; x++ )
			{
				for ( int z = 0; z < imageInfo.Width; z++ )
				{
					int location = x * imageInfo.Width + z;
					tempVertices [ index ] = new TerrainVertex (
						( x - imageInfo.Width / 2 ) * 5.0f, ( decoded [ location ].AlphaValue ) * 5.0f / 2,
						( z - imageInfo.Width / 2 ) * 5.0f,
						z / ( float ) imageInfo.Width, x / ( float ) imageInfo.Height
					);
					++index;
				}
			}
			terrainVertex.Vertices = tempVertices;

			terrainIndex = LiqueurSystem.GraphicsDevice.CreateIndexBuffer ( imageInfo.Width * imageInfo.Height * 2 * 3 );
			int [] tempIndices = new int [ terrainIndex.Length ];
			index = 0;
			for ( int z = 0; z < imageInfo.Height - 1; z++ )
			{
				for ( int x = 0; x < imageInfo.Width - 1; x++ )
				{
					tempIndices [ index++ ] = z * imageInfo.Width + x;
					tempIndices [ index++ ] = z * imageInfo.Width + ( x + 1 );
					tempIndices [ index++ ] = ( z + 1 ) * imageInfo.Width + x;
					tempIndices [ index++ ] = ( z + 1 ) * imageInfo.Width + x;
					tempIndices [ index++ ] = z * imageInfo.Width + ( x + 1 );
					tempIndices [ index++ ] = ( z + 1 ) * imageInfo.Width + ( x + 1 );
				}
			}
			terrainIndex.Indices = tempIndices;

			terrainEffect = new SpriteEffect ();

			texture = contentManager.Load<ITexture2D> ( "Test.Game.Terrain.Resources.terrain_02.png" );

			LiqueurSystem.GraphicsDevice.CullingMode = CullingMode.None;

			base.Intro ( args );
		}

		public override void Outro ()
		{
			terrainVertex.Dispose ();
			terrainIndex.Dispose ();
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
			LiqueurSystem.GraphicsDevice.BeginScene ();

			terrainEffect.SetArgument<Matrix4x4> ( "worldMatrix",
				new World3 ( new Vector3 ( 0, -200, 0 ), new Vector3 ( 0 ), new Vector3 ( 1 ), new Vector3 ( 0 ), new Vector3 ( 0 ) ).Matrix *
				new View ( new Vector3 ( 0, 0, 0 ), new Vector3 ( 0, -200, 0 ), new Vector3 ( 0, 1, 0 ) ).Matrix );
			terrainEffect.SetArgument<Matrix4x4> ( "projectionMatrix",
				new PerspectiveFieldOfViewProjection ( 3.141492f / 4, 4 / 3.0f, 0.0001f, 1000.0f ).Matrix );
			terrainEffect.SetTexture ( new TextureArgument () { Texture = texture, Uniform = "texture0" } );
			terrainEffect.Dispatch ( ( IEffect effect ) =>
			{
				LiqueurSystem.GraphicsDevice.Draw<TerrainVertex> ( PrimitiveType.TriangleList, terrainVertex, terrainIndex );
			} );

			base.Draw ( gameTime );
			LiqueurSystem.GraphicsDevice.EndScene ();
		}
    }
}
