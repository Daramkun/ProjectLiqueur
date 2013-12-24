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
				Color [] colours = new Color [ stream.Length / 4 ];
				for ( int i = 0; i < colours.Length; ++i )
					colours [ i ] = new Color ( stream.Read<SharpDX.Color> ().ToBgra (), true );
				texture.UnlockRectangle ( 0 );
				return colours;
			}
			set
			{
				SharpDX.DataRectangle dr = texture.LockRectangle ( 0, new SharpDX.Rectangle ( 0, 0, Width, Height ), SharpDX.Direct3D9.LockFlags.None );
				SharpDX.DataStream stream = new SharpDX.DataStream ( dr.DataPointer, Width * Height * 4, false, true );
				SharpDX.Color [] colours = new SharpDX.Color [ stream.Length / 4 ];
				for ( int i = 0; i < value.Length; ++i )
					colours [ i ] = new SharpDX.Color ( value [ i ].ARGBValue );
				stream.WriteRange<SharpDX.Color> ( colours );
				texture.UnlockRectangle ( 0 );
			}
		}

		public Texture2D ( IGraphicsDevice graphicsDevice, int width, int height )
		{
			if ( width == 0 ) width = 1;
			if ( height == 0 ) height = 1;
			texture = new SharpDX.Direct3D9.Texture ( graphicsDevice.Handle as SharpDX.Direct3D9.Device, width, height,
				1, SharpDX.Direct3D9.Usage.AutoGenerateMipMap, SharpDX.Direct3D9.Format.A8R8G8B8, SharpDX.Direct3D9.Pool.Managed );
			texture.FilterTexture ( 0, SharpDX.Direct3D9.Filter.Point );
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
