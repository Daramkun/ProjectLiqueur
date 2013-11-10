using System;
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

		public int Length { get; private set; }
		public int TotalBytesize { get; private set; }
		public object Handle { get { return vertexBuffer; } }
		public FlexibleVertexFormat FVF { get; private set; }

		public T [] Vertices
		{
			get
			{
				SharpDX.DataStream stream = vertexBuffer.Lock ( 0, TotalBytesize, SharpDX.Direct3D9.LockFlags.None );
				T [] arr = stream.ReadRange<T> ( Length );
				vertexBuffer.Unlock ();
				return arr;
			}
			set
			{
				SharpDX.DataStream stream = vertexBuffer.Lock ( 0, TotalBytesize, SharpDX.Direct3D9.LockFlags.None );
				stream.WriteRange<T> ( value );
				vertexBuffer.Unlock ();
			}
		}

		public VertexBuffer ( IGraphicsDevice graphicsDevice, FlexibleVertexFormat fvf, int vertexCount )
		{
			vertexBuffer = new SharpDX.Direct3D9.VertexBuffer ( graphicsDevice.Handle as SharpDX.Direct3D9.Device,
				TotalBytesize = vertexCount * Marshal.SizeOf ( typeof ( T ) ), SharpDX.Direct3D9.Usage.None, ConvertVertexFormat ( fvf ),
				SharpDX.Direct3D9.Pool.Default );
			Length = vertexCount;
			FVF = fvf;
		}

		private SharpDX.Direct3D9.VertexFormat ConvertVertexFormat ( FlexibleVertexFormat fvf )
		{
			SharpDX.Direct3D9.VertexFormat vf = SharpDX.Direct3D9.VertexFormat.None;
			
			if ( fvf.HasFlag ( FlexibleVertexFormat.PositionXY ) ) vf |= SharpDX.Direct3D9.VertexFormat.Position;
			if ( fvf.HasFlag ( FlexibleVertexFormat.PositionXYZ ) ) vf |= SharpDX.Direct3D9.VertexFormat.Position;

			if ( fvf.HasFlag ( FlexibleVertexFormat.Normal ) ) vf |= SharpDX.Direct3D9.VertexFormat.Normal;

			if ( fvf.HasFlag ( FlexibleVertexFormat.Diffuse ) ) vf |= SharpDX.Direct3D9.VertexFormat.Diffuse;

			if ( fvf.HasFlag ( FlexibleVertexFormat.TextureUV1 ) ) vf |= SharpDX.Direct3D9.VertexFormat.Texture1;
			if ( fvf.HasFlag ( FlexibleVertexFormat.TextureUV2 ) ) vf |= SharpDX.Direct3D9.VertexFormat.Texture2;
			if ( fvf.HasFlag ( FlexibleVertexFormat.TextureUV3 ) ) vf |= SharpDX.Direct3D9.VertexFormat.Texture3;
			if ( fvf.HasFlag ( FlexibleVertexFormat.TextureUV4 ) ) vf |= SharpDX.Direct3D9.VertexFormat.Texture4;

			return vf;
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
