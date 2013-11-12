using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramkun.Liqueur.Graphics
{
	class IndexBuffer : IIndexBuffer
	{
		SharpDX.Direct3D9.IndexBuffer indexBuffer;

		public int Length { get; private set; }

		public int [] Indices
		{
			get
			{
				SharpDX.DataStream stream = indexBuffer.Lock ( 0, Length * 4, SharpDX.Direct3D9.LockFlags.None );
				int [] arr = stream.ReadRange<int> ( Length );
				indexBuffer.Unlock ();
				return arr;
			}
			set
			{
				SharpDX.DataStream stream = indexBuffer.Lock ( 0, Length * 4, SharpDX.Direct3D9.LockFlags.None );
				stream.WriteRange<int> ( value );
				indexBuffer.Unlock ();
			}
		}

		public object Handle { get { return indexBuffer; } }

		public IndexBuffer ( IGraphicsDevice graphicsDevice, int indexCount )
		{
			Length = indexCount;
			indexBuffer = new SharpDX.Direct3D9.IndexBuffer ( graphicsDevice.Handle as SharpDX.Direct3D9.Device,
				indexCount * 4, SharpDX.Direct3D9.Usage.None, SharpDX.Direct3D9.Pool.Managed, false );
		}

		public IndexBuffer ( IGraphicsDevice graphicsDevice, int [] indices )
			: this ( graphicsDevice, indices.Length )
		{
			Indices = indices;
		}

		public void Dispose ()
		{
			indexBuffer.Dispose ();
		}
	}
}
