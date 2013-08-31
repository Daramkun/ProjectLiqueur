using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	class VertexBuffer<T> : IVertexBuffer<T> where T : struct
	{
		Microsoft.Xna.Framework.Graphics.VertexBuffer vertexBuffer;

		public int Length { get { return vertexBuffer.VertexCount; } }
		public int TotalBytesize { get { throw new NotImplementedException (); } }

		public object Handle { get { return vertexBuffer; } }

		public FlexibleVertexFormat FVF
		{
			get { throw new NotImplementedException (); }
		}

		public T [] Vertices
		{
			get
			{
				throw new NotImplementedException ();
			}
			set
			{
				throw new NotImplementedException ();
			}
		}

		public VertexBuffer ( IGraphicsDevice graphicsDevice, FlexibleVertexFormat fvf, int vertexCount )
		{
			vertexBuffer = new Microsoft.Xna.Framework.Graphics.VertexBuffer ( ( graphicsDevice.Handle as Microsoft.Xna.Framework.Graphics.GraphicsDevice ),
				new Microsoft.Xna.Framework.Graphics.VertexDeclaration (), vertexCount, Microsoft.Xna.Framework.Graphics.BufferUsage.None );
		}

		public VertexBuffer ( IGraphicsDevice graphicsDevice, FlexibleVertexFormat fvf, T [] vertices )
			: this ( graphicsDevice, fvf, vertices.Length )
		{
			Vertices = vertices;
		}

		public void Dispose ()
		{

		}
	}
}
