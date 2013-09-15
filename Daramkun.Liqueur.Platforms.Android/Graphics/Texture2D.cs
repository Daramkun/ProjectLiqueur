using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Mathematics;
using OpenTK.Graphics.ES20;

namespace Daramkun.Liqueur.Graphics
{
	class Texture2D : ITexture2D
	{
		internal int texture;

		public int Width { get { return ( int ) Size.X; } }
		public int Height { get { return ( int ) Size.Y; } }

		public object Handle { get { return texture; } }

		public Vector2 Size { get; private set; }

		public Color [] Buffer
		{
			get
			{
				Color [] pixels = new Color [ Width * Height ];
				GL.ReadPixels<Color> ( 0, 0, Width, Height, All.Rgba, All.UnsignedInt, pixels );
				return pixels;
			}
			set
			{
				GL.BindTexture ( All.Texture2D, texture );
				
				GL.TexParameter ( All.Texture2D, All.TextureMagFilter, ( int ) All.Linear );
				GL.TexParameter ( All.Texture2D, All.TextureMinFilter, ( int ) All.Linear );
				GL.TexParameter ( All.Texture2D, All.TextureWrapS, ( int ) All.ClampToEdge );
				GL.TexParameter ( All.Texture2D, All.TextureWrapT, ( int ) All.ClampToEdge );

				byte [] colorData = new byte [ Width * Height * 4 ];

				for ( int i = 0, index = 0; i < value.Length; i++ )
				{
					colorData [ index++ ] = value [ i ].BlueValue;
					colorData [ index++ ] = value [ i ].GreenValue;
					colorData [ index++ ] = value [ i ].RedValue;
					colorData [ index++ ] = value [ i ].AlphaValue;
				}

				GL.TexImage2D<byte> ( All.Texture2D, 0, 32856,
				                     Width, Height, 0, All.BgraImg,
				                     All.UnsignedByte, colorData );

				GL.BindTexture ( All.Texture2D, 0 );
			}
		}

		public Texture2D ( IGraphicsDevice graphicsDevice, int width, int height )
		{
			Size = new Vector2 ( width, height );
			texture = GL.GenTexture ();
			GL.BindTexture ( All.Texture2D, texture );
			GL.TexImage2D ( All.Texture2D, 0, 32856, width, height, 0, All.BgraImg, All.UnsignedByte, IntPtr.Zero );
			GL.BindTexture ( All.Texture2D, 0 );
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
				if ( texture == 0 )
					return;
				GL.DeleteTexture ( texture );
				texture = 0;
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}
	}
}
