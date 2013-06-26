using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Graphics.Vertices;
using Daramkun.Liqueur.Math;
using OpenTK.Graphics.OpenGL;

namespace Daramkun.Liqueur.Graphics
{
	class Primitive<T> : IPrimitive<T> where T : IFlexibleVertex
	{
		FlexibleVertexArray<T> vertexBuffer;
		int [] indexBuffer;

		internal float [] vertexArray;
		internal float [] normalArray;
		internal float [] colorArray;
		internal float [] textureArray;
		internal float [] subTextureArray;

		public FlexibleVertexArray<T> Vertices
		{
			get { return vertexBuffer; }
		}

		public int [] Indices
		{
			get { return indexBuffer; }
		}

		public PrimitiveType PrimitiveType { get; set; }
		public int PrimitiveCount { get; set; }
		public ITexture2D Texture { get; set; }
		public ITexture2D SubTexture { get; set; }

		public Primitive ( int vertexCount, int indexCount )
		{
			vertexBuffer = new FlexibleVertexArray<T> ( vertexCount );
			if ( indexCount != 0 )
				indexBuffer = new int [ indexCount ];

			SetupPrimitive ();
		}

		public Primitive ( T [] vertices, int [] indices )
		{
			vertexBuffer = new FlexibleVertexArray<T> ( vertices );
			if ( indices != null && indices.Length != 0 )
			{
				indexBuffer = new int [ indices.Length ];
				indices.CopyTo ( indexBuffer, 0 );
			}

			SetupPrimitive ();
		}

		public void Dispose ()
		{
			vertexBuffer = null;
			indexBuffer = null;

			vertexArray = null;
			normalArray = null;
			textureArray = null;
			colorArray = null;
		}

		private void SetupPrimitive ()
		{
			vertexBuffer.ValueChanged += ( object sender, ArrayValueChangedEventArgs e ) =>
			{
				SetBuffer ( vertexBuffer [ e.Index ], e.Index );
			};

			if ( Utilities.IsSubtypeOf ( typeof ( T ), typeof ( IFlexibleVertexPositionXY ) ) )
				vertexArray = new float [ vertexBuffer.Length * 2 ];
			else if ( Utilities.IsSubtypeOf ( typeof ( T ), typeof ( IFlexibleVertexPositionXYZ ) ) )
				vertexArray = new float [ vertexBuffer.Length * 3 ];

			if ( Utilities.IsSubtypeOf ( typeof ( T ), typeof ( IFlexibleVertexNormal ) ) )
				normalArray = new float [ vertexBuffer.Length * 3 ];

			if ( Utilities.IsSubtypeOf ( typeof ( T ), typeof ( IFlexibleVertexDiffuse ) ) )
				colorArray = new float [ vertexBuffer.Length * 4 ];

			if ( Utilities.IsSubtypeOf ( typeof ( T ), typeof ( IFlexibleVertexTexture1 ) ) )
				textureArray = new float [ vertexBuffer.Length * 2 ];

			if ( Utilities.IsSubtypeOf ( typeof ( T ), typeof ( IFlexibleVertexTexture2 ) ) )
				subTextureArray = new float [ vertexBuffer.Length * 2 ];

			int index = 0;
			foreach ( T vertex in Vertices )
			{
				SetBuffer ( vertex, index );
				++index;
			}
		}

		private void SetBuffer ( T vertex, int index )
		{
			if ( vertex is IFlexibleVertexPositionXY )
			{
				Vector2 vec = ( vertex as IFlexibleVertexPositionXY ).Position;
				vertexArray [ index * 2 + 0 ] = vec.X;
				vertexArray [ index * 2 + 1 ] = vec.Y;
			}
			else if ( vertex is IFlexibleVertexPositionXYZ )
			{
				Vector3 vec = ( vertex as IFlexibleVertexPositionXYZ ).Position;
				vertexArray [ index * 3 + 0 ] = vec.X;
				vertexArray [ index * 3 + 1 ] = vec.Y;
				vertexArray [ index * 3 + 2 ] = vec.Z;
			}

			if ( vertex is IFlexibleVertexNormal )
			{
				Vector3 vec = ( vertex as IFlexibleVertexNormal ).Normal;
				normalArray [ index * 3 + 0 ] = vec.X;
				normalArray [ index * 3 + 1 ] = vec.Y;
				normalArray [ index * 3 + 2 ] = vec.Z;
			}

			if ( vertex is IFlexibleVertexTexture1 )
			{
				Vector2 vec = ( vertex as IFlexibleVertexTexture1 ).TextureUV1;
				textureArray [ index * 2 + 0 ] = vec.X;
				textureArray [ index * 2 + 1 ] = vec.Y;
			}

			if ( vertex is IFlexibleVertexTexture2 )
			{
				Vector2 vec = ( vertex as IFlexibleVertexTexture2 ).TextureUV2;
				subTextureArray [ index * 2 + 0 ] = vec.X;
				subTextureArray [ index * 2 + 1 ] = vec.Y;
			}

			if ( vertex is IFlexibleVertexDiffuse )
			{
				Color col = ( vertex as IFlexibleVertexDiffuse ).Diffuse;
				colorArray [ index * 4 + 0 ] = col.RedScalar;
				colorArray [ index * 4 + 1 ] = col.GreenScalar;
				colorArray [ index * 4 + 2 ] = col.BlueScalar;
				colorArray [ index * 4 + 3 ] = col.AlphaScalar;
			}
		}
	}
}
