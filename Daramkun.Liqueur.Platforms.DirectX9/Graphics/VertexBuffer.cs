﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Daramkun.Liqueur.Graphics
{
	class VertexBuffer<T> : IVertexBuffer<T> where T : struct
	{
		SharpDX.Direct3D9.VertexBuffer vertexBuffer;
		internal int typeSize;
		internal SharpDX.Direct3D9.VertexDeclaration vertexDeclaration;
		
		public int Length { get; private set; }
		public int TotalBytesize { get { return typeSize * Length; } }
		public object Handle { get { return vertexBuffer; } }
		public FlexibleVertexFormat FVF { get; private set; }

		public T [] Vertices
		{
			get
			{
				SharpDX.DataStream stream = vertexBuffer.Lock ( 0, 0, SharpDX.Direct3D9.LockFlags.None );
				T [] arr = stream.ReadRange<T> ( Length );
				vertexBuffer.Unlock ();
				return arr;
			}
			set
			{
				SharpDX.DataStream stream = vertexBuffer.Lock ( 0, 0, SharpDX.Direct3D9.LockFlags.None );
				stream.WriteRange<T> ( value );
				vertexBuffer.Unlock ();
			}
		}

		public VertexBuffer ( IGraphicsDevice graphicsDevice, FlexibleVertexFormat fvf, int vertexCount )
		{
			Length = vertexCount;
			FVF = fvf;
			typeSize = Marshal.SizeOf ( typeof ( T ) );

			SharpDX.Direct3D9.Device device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			vertexBuffer = new SharpDX.Direct3D9.VertexBuffer ( device, TotalBytesize, SharpDX.Direct3D9.Usage.None,
				0, SharpDX.Direct3D9.Pool.Managed );

			vertexDeclaration = new SharpDX.Direct3D9.VertexDeclaration ( device, ConvertVertexElements ( fvf ) );
		}

		private SharpDX.Direct3D9.VertexElement [] ConvertVertexElements ( FlexibleVertexFormat fvf )
		{
			List<SharpDX.Direct3D9.VertexElement> elements = new List<SharpDX.Direct3D9.VertexElement> ();

			short offset = 0;

			if ( fvf.HasFlag ( FlexibleVertexFormat.PositionXY ) )
			{
				elements.Add ( new SharpDX.Direct3D9.VertexElement ( 0, offset, SharpDX.Direct3D9.DeclarationType.Float2,
					SharpDX.Direct3D9.DeclarationMethod.Default, SharpDX.Direct3D9.DeclarationUsage.Position, 0 ) );
				offset += 8;
			}
			else if ( fvf.HasFlag ( FlexibleVertexFormat.PositionXYZ ) )
			{
				elements.Add ( new SharpDX.Direct3D9.VertexElement ( 0, offset, SharpDX.Direct3D9.DeclarationType.Float3,
					SharpDX.Direct3D9.DeclarationMethod.Default, SharpDX.Direct3D9.DeclarationUsage.Position, 0 ) );
				offset += 12;
			}

			if ( fvf.HasFlag ( FlexibleVertexFormat.Diffuse ) )
			{
				elements.Add ( new SharpDX.Direct3D9.VertexElement ( 0, offset, SharpDX.Direct3D9.DeclarationType.Float4,
					SharpDX.Direct3D9.DeclarationMethod.Default, SharpDX.Direct3D9.DeclarationUsage.Color, 0 ) );
				offset += 16;
			}

			if ( fvf.HasFlag ( FlexibleVertexFormat.Normal ) )
			{
				elements.Add ( new SharpDX.Direct3D9.VertexElement ( 0, offset, SharpDX.Direct3D9.DeclarationType.Float3,
					SharpDX.Direct3D9.DeclarationMethod.Default, SharpDX.Direct3D9.DeclarationUsage.Normal, 0 ) );
				offset += 12;
			}

			if ( fvf.HasFlag ( FlexibleVertexFormat.TextureUV1 ) )
			{
				elements.Add ( new SharpDX.Direct3D9.VertexElement ( 0, offset, SharpDX.Direct3D9.DeclarationType.Float2,
					SharpDX.Direct3D9.DeclarationMethod.Default, SharpDX.Direct3D9.DeclarationUsage.TextureCoordinate, 0 ) );
				offset += 8;
			}

			if ( fvf.HasFlag ( FlexibleVertexFormat.TextureUV2 ) )
			{
				elements.Add ( new SharpDX.Direct3D9.VertexElement ( 0, offset, SharpDX.Direct3D9.DeclarationType.Float2,
					SharpDX.Direct3D9.DeclarationMethod.Default, SharpDX.Direct3D9.DeclarationUsage.TextureCoordinate, 1 ) );
				offset += 8;
			}

			if ( fvf.HasFlag ( FlexibleVertexFormat.TextureUV3 ) )
			{
				elements.Add ( new SharpDX.Direct3D9.VertexElement ( 0, offset, SharpDX.Direct3D9.DeclarationType.Float2,
					SharpDX.Direct3D9.DeclarationMethod.Default, SharpDX.Direct3D9.DeclarationUsage.TextureCoordinate, 2 ) );
				offset += 8;
			}

			if ( fvf.HasFlag ( FlexibleVertexFormat.TextureUV4 ) )
			{
				elements.Add ( new SharpDX.Direct3D9.VertexElement ( 0, offset, SharpDX.Direct3D9.DeclarationType.Float2,
					SharpDX.Direct3D9.DeclarationMethod.Default, SharpDX.Direct3D9.DeclarationUsage.TextureCoordinate, 3 ) );
				offset += 8;
			}
			elements.Add ( SharpDX.Direct3D9.VertexElement.VertexDeclarationEnd );

			return elements.ToArray ();
		}

		public VertexBuffer ( IGraphicsDevice graphicsDevice, FlexibleVertexFormat fvf, T [] vertices )
			: this ( graphicsDevice, fvf, vertices.Length )
		{
			Vertices = vertices;
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
