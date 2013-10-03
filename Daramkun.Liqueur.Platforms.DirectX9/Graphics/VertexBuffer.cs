using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	class VertexBuffer<T> : IVertexBuffer<T> where T : struct
	{
		SharpDX.Direct3D9.VertexBuffer vertexBuffer;

		public int Length { get; private set; }
		public int TotalBytesize { get; private set; }
		public object Handle { get { return vertexBuffer; } }
		public FlexibleVertexFormat FVF { get; private set; }

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

		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
	}
}
