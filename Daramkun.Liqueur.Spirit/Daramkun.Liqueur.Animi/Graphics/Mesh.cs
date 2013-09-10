using System;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Animi.Graphics
{
	public class Mesh<T> : IDisposable where T : struct
	{
		public PrimitiveType PrimitiveType { get; set; }
		public IVertexBuffer<T> VertexBuffer { get; private set; }
		public IIndexBuffer IndexBuffer { get; private set; }

		public Mesh ( IVertexBuffer<T> vertexBuffer, IIndexBuffer indexBuffer = null,
			PrimitiveType primitiveType = PrimitiveType.TriangleList )
		{
			VertexBuffer = vertexBuffer;
			IndexBuffer = indexBuffer;
			PrimitiveType = primitiveType;
		}

		public Mesh ( FlexibleVertexFormat fvf, PrimitiveType primitiveType = PrimitiveType.TriangleList,
			params T [] vertices )
		{
			VertexBuffer = LiqueurSystem.GraphicsDevice.CreateVertexBuffer ( fvf, vertices );
			PrimitiveType = primitiveType;
		}

		public Mesh ( FlexibleVertexFormat fvf, T [] vertices, int [] indices,
			PrimitiveType primitiveType = PrimitiveType.TriangleList )
		{
			VertexBuffer = LiqueurSystem.GraphicsDevice.CreateVertexBuffer ( fvf, vertices );
			IndexBuffer = LiqueurSystem.GraphicsDevice.CreateIndexBuffer ( indices );
			PrimitiveType = primitiveType;
		}

		~Mesh ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				if ( IndexBuffer != null )
					IndexBuffer.Dispose ();
				VertexBuffer.Dispose ();
			}
		}

		public void Dispose()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		public void Draw()
		{
			if ( IndexBuffer == null ) LiqueurSystem.GraphicsDevice.Draw ( PrimitiveType, VertexBuffer );
			else LiqueurSystem.GraphicsDevice.Draw ( PrimitiveType, VertexBuffer, IndexBuffer );
		}
	}
}