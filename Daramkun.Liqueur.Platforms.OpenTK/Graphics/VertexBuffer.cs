using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using OpenTK.Graphics.OpenGL;

namespace Daramkun.Liqueur.Graphics
{
	class VertexDeclaration
	{
		public int Size;
		public int Offset;
	}

	class VertexBuffer<T> : IVertexBuffer<T> where T : struct
	{
		internal int vertexBuffer;
		internal VertexDeclaration [] declaration;
		internal int typeSize;

		public int Length { get; private set; }

		public int TotalBytesize { get { return typeSize * Length; } }

		public FlexibleVertexFormat FVF { get; private set; }

		public object Handle { get { return vertexBuffer; } }

		public T [] Vertices
		{
			get
			{
				GL.BindBuffer ( BufferTarget.ArrayBuffer, vertexBuffer );
				T [] data = new T [ Length ];
				IntPtr unmanagedHandle = GL.MapBuffer ( BufferTarget.ArrayBuffer, BufferAccess.ReadOnly );
				for ( int i = 0; i < Length; i++ )
				{
					data [ i ] = ( T ) Marshal.PtrToStructure ( unmanagedHandle, typeof ( T ) );
					unmanagedHandle += TotalBytesize / Length;
				}
				GL.UnmapBuffer ( BufferTarget.ArrayBuffer );
				GL.BindBuffer ( BufferTarget.ArrayBuffer, 0 );
				return data;
			}
			set
			{
				GL.BindBuffer ( BufferTarget.ArrayBuffer, vertexBuffer );
				GL.BufferSubData<T> ( BufferTarget.ArrayBuffer, new IntPtr ( 0 ), new IntPtr ( TotalBytesize ), value );
				GL.BindBuffer ( BufferTarget.ArrayBuffer, 0 );
			}
		}

		public VertexBuffer ( IGraphicsDevice graphicsDevice, FlexibleVertexFormat fvf, int vertexCount )
		{
			FVF = fvf;
			Length = vertexCount;

			typeSize = Marshal.SizeOf ( typeof ( T ) );
			DetectDeclaration ( fvf );

			GL.GenBuffers ( 1, out vertexBuffer );

			GL.BindBuffer ( BufferTarget.ArrayBuffer, vertexBuffer );
			GL.BufferData ( BufferTarget.ArrayBuffer, new IntPtr ( TotalBytesize ), IntPtr.Zero, BufferUsageHint.StaticDraw );
			GL.BindBuffer ( BufferTarget.ArrayBuffer, 0 );
		}

		private void DetectDeclaration ( FlexibleVertexFormat fvf )
		{
			List<VertexDeclaration> _declaration = new List<VertexDeclaration> ();
			int offset = 0;
			if ( ( fvf & FlexibleVertexFormat.PositionXY ) != 0 )
			{
				_declaration.Add ( new VertexDeclaration () { Size = 2, Offset = offset } );
				offset += 8;
			}
			else if ( ( fvf & FlexibleVertexFormat.PositionXYZ ) != 0 )
			{
				_declaration.Add ( new VertexDeclaration () { Size = 3, Offset = offset } );
				offset += 12;
			}

			if ( ( fvf & FlexibleVertexFormat.Diffuse ) != 0 )
			{
				_declaration.Add ( new VertexDeclaration () { Size = 4, Offset = offset } );
				offset += 16;
			}

			if ( ( fvf & FlexibleVertexFormat.Normal ) != 0 )
			{
				_declaration.Add ( new VertexDeclaration () { Size = 3, Offset = offset } );
				offset += 12;
			}

			if ( ( fvf & FlexibleVertexFormat.TextureUV1 ) != 0 )
			{
				_declaration.Add ( new VertexDeclaration () { Size = 2, Offset = offset } );
				offset += 8;
			}

			if ( ( fvf & FlexibleVertexFormat.TextureUV2 ) != 0 )
			{
				_declaration.Add ( new VertexDeclaration () { Size = 2, Offset = offset } );
				offset += 8;
			}

			if ( ( fvf & FlexibleVertexFormat.TextureUV3 ) != 0 )
			{
				_declaration.Add ( new VertexDeclaration () { Size = 2, Offset = offset } );
				offset += 8;
			}

			if ( ( fvf & FlexibleVertexFormat.TextureUV4 ) != 0 )
			{
				_declaration.Add ( new VertexDeclaration () { Size = 2, Offset = offset } );
				offset += 8;
			}

			declaration = _declaration.ToArray ();
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
