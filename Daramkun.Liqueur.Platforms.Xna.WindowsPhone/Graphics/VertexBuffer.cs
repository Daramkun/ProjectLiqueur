using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	class VertexBuffer<T> : IVertexBuffer<T> where T : struct
	{
		Microsoft.Xna.Framework.Graphics.VertexBuffer vertexBuffer;

		public int Length { get { return vertexBuffer.VertexCount; } }
		public int TotalBytesize { get { return Marshal.SizeOf ( typeof ( T ) ) * Length; } }

		public object Handle { get { return vertexBuffer; } }

		public FlexibleVertexFormat FVF { get; private set; }

		public T [] Vertices
		{
			get
			{
				T [] buffer = new T [ Length ];
				vertexBuffer.GetData<T> ( buffer );
				return buffer;
			}
			set { vertexBuffer.SetData<T> ( value ); }
		}

		public VertexBuffer ( IGraphicsDevice graphicsDevice, FlexibleVertexFormat fvf, int vertexCount )
		{
			FVF = fvf;
			vertexBuffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer ( ( graphicsDevice.Handle as Microsoft.Xna.Framework.Graphics.GraphicsDevice ),
				new Microsoft.Xna.Framework.Graphics.VertexDeclaration (), vertexCount, Microsoft.Xna.Framework.Graphics.BufferUsage.None );
		}

		public VertexBuffer ( IGraphicsDevice graphicsDevice, FlexibleVertexFormat fvf, T [] vertices )
			: this ( graphicsDevice, fvf, vertices.Length )
		{
			Vertices = vertices;
		}

		~VertexBuffer ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				vertexBuffer.Dispose ();
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}
	}
}
