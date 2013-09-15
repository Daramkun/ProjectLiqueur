using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Graphics.ES20;

namespace Daramkun.Liqueur.Graphics
{
	class VertexBuffer<T> : IVertexBuffer<T> where T : struct
	{
		internal int vertexBuffer;

		public int Length { get; private set; }

		public int TotalBytesize { get { return Marshal.SizeOf ( typeof ( T ) ) * Length; } }

		public FlexibleVertexFormat FVF { get; private set; }

		public object Handle { get { return vertexBuffer; } }

		public T [] Vertices
		{
			get
			{
				GL.BindBuffer ( All.ArrayBuffer, vertexBuffer );
				T [] data = new T [ Length ];
				IntPtr unmanagedHandle = GL.Oes.MapBuffer ( All.ArrayBuffer, ( All ) 35000 );
				for ( int i = 0; i < Length; i++ )
				{
					data [ i ] = ( T ) Marshal.PtrToStructure ( unmanagedHandle, typeof ( T ) );
					unmanagedHandle += TotalBytesize / Length;
				}
				GL.Oes.UnmapBuffer ( All.ArrayBuffer );
				GL.BindBuffer ( All.ArrayBuffer, 0 );
				return data;
			}
			set
			{
				GL.BindBuffer ( All.ArrayBuffer, vertexBuffer );
				GL.BufferSubData<T> ( All.ArrayBuffer, new IntPtr ( 0 ), new IntPtr ( TotalBytesize ), value );
				GL.BindBuffer ( All.ArrayBuffer, 0 );
			}
		}

		public VertexBuffer ( IGraphicsDevice graphicsDevice, FlexibleVertexFormat fvf, int vertexCount )
		{
			FVF = fvf;
			Length = vertexCount;

			GL.GenBuffers ( 1, out vertexBuffer );

			GL.BindBuffer ( All.ArrayBuffer, vertexBuffer );
			GL.BufferData ( All.ArrayBuffer, new IntPtr ( TotalBytesize ), IntPtr.Zero, All.StaticDraw );
			GL.BindBuffer ( All.ArrayBuffer, 0 );
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
				GL.DeleteBuffers ( 1, ref vertexBuffer );
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}
	}
}
