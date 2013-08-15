using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace Daramkun.Liqueur.Graphics
{
	class IndexBuffer : IIndexBuffer
	{
		internal int indexBuffer;

		public int Length { get; private set; }

		public int [] Indices
		{
			get
			{
				GL.BindBuffer ( BufferTarget.ElementArrayBuffer, indexBuffer );
				int [] data = new int [ Length ];
				IntPtr unmanagedHandle = GL.MapBuffer ( BufferTarget.ElementArrayBuffer, BufferAccess.ReadOnly );
				Marshal.Copy ( unmanagedHandle, data, 0, Length );
				GL.UnmapBuffer ( BufferTarget.ElementArrayBuffer );
				GL.BindBuffer ( BufferTarget.ElementArrayBuffer, 0 );
				return data;
			}
			set
			{
				GL.BindBuffer ( BufferTarget.ElementArrayBuffer, indexBuffer );
				GL.BufferSubData<int> ( BufferTarget.ElementArrayBuffer, new IntPtr ( 0 ), new IntPtr ( Length * sizeof ( int ) ), value );
				GL.BindBuffer ( BufferTarget.ElementArrayBuffer, 0 );
			}
		}

		public object Handle { get { return indexBuffer; } }

		public IndexBuffer ( int indexCount )
		{
			Length = indexCount;

			GL.GenBuffers ( 1, out indexBuffer );

			GL.BindBuffer ( BufferTarget.ElementArrayBuffer, indexBuffer );
			GL.BufferData ( BufferTarget.ElementArrayBuffer, new IntPtr ( Length * sizeof ( int ) ), IntPtr.Zero, BufferUsageHint.StaticDraw );
			GL.BindBuffer ( BufferTarget.ElementArrayBuffer, 0 );
		}

		public IndexBuffer ( IGraphicsDevice graphicsDevice, int [] indices )
			: this ( indices.Length )
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
				GL.DeleteBuffers ( 1, ref indexBuffer );
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}
	}
}