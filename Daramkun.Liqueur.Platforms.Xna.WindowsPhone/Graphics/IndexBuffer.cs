using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	class IndexBuffer : IIndexBuffer
	{
		Microsoft.Xna.Framework.Graphics.IndexBuffer indexBuffer;

		public int Length { get; private set; }

		public int [] Indices
		{
			get
			{
				int [] buffer = new int [ Length ];
				indexBuffer.GetData<int> ( buffer );
				return buffer;
			}
			set { indexBuffer.SetData<int> ( value ); }
		}

		public object Handle { get { return indexBuffer; } }

		public IndexBuffer ( IGraphicsDevice graphicsDevice, int indexCount )
		{
			indexBuffer = new Microsoft.Xna.Framework.Graphics.IndexBuffer ( graphicsDevice.Handle as Microsoft.Xna.Framework.Graphics.GraphicsDevice,
				typeof ( int ), indexCount, Microsoft.Xna.Framework.Graphics.BufferUsage.None );
			Length = indexCount;
		}

		public IndexBuffer ( IGraphicsDevice graphicsDevice, int [] indices )
			: this ( graphicsDevice, indices.Length )
		{
			Indices = indices;
		}

		~IndexBuffer ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				indexBuffer.Dispose ();
				indexBuffer = null;
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}
	}
}
