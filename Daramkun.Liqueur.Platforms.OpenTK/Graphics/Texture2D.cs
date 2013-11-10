using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Mathematics;
using OpenTK.Graphics.OpenGL;

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
				GL.ReadPixels<Color> ( 0, 0, Width, Height, PixelFormat.Rgba, PixelType.UnsignedInt8888, pixels );
				return pixels;
			}
			set
			{
				GL.BindTexture ( TextureTarget.Texture2D, texture );

				GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ( int ) TextureMagFilter.Nearest );
				GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ( int ) TextureMinFilter.Nearest );
				GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ( int ) TextureWrapMode.Repeat );
				GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ( int ) TextureWrapMode.Repeat );

				byte [] colorData = new byte [ Width * Height * 4 ];

				for ( int i = 0, index = 0; i < value.Length; i++ )
				{
					colorData [ index++ ] = value [ i ].BlueValue;
					colorData [ index++ ] = value [ i ].GreenValue;
					colorData [ index++ ] = value [ i ].RedValue;
					colorData [ index++ ] = value [ i ].AlphaValue;
				}

				GL.TexImage2D<byte> ( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8,
					Width, Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
					PixelType.UnsignedByte, colorData );

				GL.BindTexture ( TextureTarget.Texture2D, 0 );
			}
		}

		public Texture2D ( IGraphicsDevice graphicsDevice, int width, int height )
		{
			Size = new Vector2 ( width, height );
			texture = GL.GenTexture ();
			GL.BindTexture ( TextureTarget.Texture2D, texture );
			GL.TexImage2D ( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8, width, height, 0, PixelFormat.Bgra, PixelType.UnsignedByte, IntPtr.Zero );
			GL.BindTexture ( TextureTarget.Texture2D, 0 );
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
