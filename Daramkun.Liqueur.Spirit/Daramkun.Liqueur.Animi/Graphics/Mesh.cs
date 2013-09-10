using System;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Animi
{
	public class Mesh<T> : IDisposable where T : struct
	{
		public FlexibleVertexFormat FVF{ get; private set; }
		public IVertexBuffer<T> VertexBuffer { get; private set; }
		public IIndexBuffer IndexBuffer { get; private set; }

		public Mesh ( FlexibleVertexFormat fvf, IVertexBuffer<T> vertexBuffer, IIndexBuffer indexBuffer = null )
		{
			FVF = fvf;
			VertexBuffer = vertexBuffer;
			IndexBuffer = indexBuffer;
		}

		public Mesh ( FlexibleVertexFormat fvf, params T [] vertices )
		{
			FVF = fvf;
			VertexBuffer = LiqueurSystem.GraphicsDevice.CreateVertexBuffer ( fvf, vertices );
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
			GC.SuppressFinalize ();
		}
	}
}

