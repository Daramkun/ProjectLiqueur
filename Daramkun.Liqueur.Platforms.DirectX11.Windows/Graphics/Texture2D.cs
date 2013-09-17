using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Mathematics;
using SharpDX.Direct3D11;

namespace Daramkun.Liqueur.Graphics
{
	class Texture2D : ITexture2D
	{
		SharpDX.Direct3D11.DeviceContext deviceContext;
		SharpDX.Direct3D11.Texture2D texture;

		public int Width { get { throw new NotImplementedException (); } }
		public int Height { get { throw new NotImplementedException (); } }

		public Vector2 Size
		{
			get { throw new NotImplementedException (); }
		}

		public Color [] Buffer
		{
			get
			{
				SharpDX.DataBox box = deviceContext.MapSubresource ( texture, 0, MapMode.Read, MapFlags.None );
				SharpDX.DataStream stream = new SharpDX.DataStream ( box.DataPointer, Width * Height * 4 * 4, true, false );
				Color [] color = new Color [ Width * Height ];
				int index = 0;
				while ( stream.Position != stream.Length )
					color [ index++ ] = stream.Read<Color> ();
				deviceContext.UnmapSubresource ( texture, 0 );
				return color;
			}
			set
			{
				SharpDX.DataBox box = deviceContext.MapSubresource ( texture, 0, MapMode.Write, MapFlags.None );
				SharpDX.DataStream stream = new SharpDX.DataStream ( box.DataPointer, Width * Height * 4 * 4, true, false );
				foreach ( Color c in value )
					stream.Write<Color> ( c );
				deviceContext.UnmapSubresource ( texture, 0 );
			}
		}

		public object Handle { get { return texture; } }

		public Texture2D ( IGraphicsDevice graphicsDevice, ImageInfo imageInfo, Color? colorKey )
		{
			deviceContext = ( graphicsDevice as GraphicsDevice ).d3dContext;
			texture = new SharpDX.Direct3D11.Texture2D ( graphicsDevice.Handle as SharpDX.Direct3D11.Device,
				new SharpDX.Direct3D11.Texture2DDescription ()
				{
					Width = imageInfo.Width,
					Height = imageInfo.Height,
					SampleDescription = new SharpDX.DXGI.SampleDescription ( 1, 0 ),
					Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm,
					ArraySize = 1,
					MipLevels = 1,
				} );
			Buffer = imageInfo.GetPixel ( colorKey );
		}

		~Texture2D ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				texture.Dispose ();
				texture = null;
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}
	}
}
