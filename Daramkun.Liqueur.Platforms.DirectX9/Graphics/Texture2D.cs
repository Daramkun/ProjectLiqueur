using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Graphics
{
	class Texture2D : ITexture2D
	{
		internal SharpDX.Direct3D9.Texture texture;

		public int Width { get { return ( int ) Size.X; } }
		public int Height { get { return ( int ) Size.Y; } }

		public object Handle { get { return texture; } }

		public Vector2 Size { get; private set; }

		public Color [] Buffer
		{
			get
			{
				SharpDX.DataRectangle dr = texture.LockRectangle ( 0, new SharpDX.Rectangle ( 0, 0, Width, Height ), SharpDX.Direct3D9.LockFlags.None );
				SharpDX.DataStream stream = new SharpDX.DataStream ( dr.DataPointer, Width * Height * 4, true, false );
				Color [] colours = new Color [ Width * Height ];
				for ( int i = 0; i < colours.Length; ++i )
					colours [ i ] = new Color ( ( uint ) stream.ReadInt () );
				return colours;
			}
			set
			{
				SharpDX.DataRectangle dr = texture.LockRectangle ( 0, new SharpDX.Rectangle ( 0, 0, Width, Height ), SharpDX.Direct3D9.LockFlags.None );
				SharpDX.DataStream stream = new SharpDX.DataStream ( dr.DataPointer, Width * Height * 4, false, true );
				for ( int i = 0; i < value.Length; ++i )
					stream.Write<uint> ( value [ i ].ARGBColorValue );
			}
		}

		public Texture2D ( IGraphicsDevice graphicsDevice, int width, int height )
		{
			texture = new SharpDX.Direct3D9.Texture ( graphicsDevice.Handle as SharpDX.Direct3D9.Device, width, height, 1, SharpDX.Direct3D9.Usage.None,
				SharpDX.Direct3D9.Format.A8R8G8B8, SharpDX.Direct3D9.Pool.Managed );
			Size = new Vector2 ( width, height );
		}

		public Texture2D ( IGraphicsDevice graphicsDevice, ImageInfo imageInfo, Color? colorKey = null )
			: this ( graphicsDevice, imageInfo.Width, imageInfo.Height )
		{
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
