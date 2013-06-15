using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Decoders;
using Daramkun.Liqueur.Decoders.Images;
using Daramkun.Liqueur.Geometries;
#if OPENTK
using OpenTK.Graphics.OpenGL;
#elif XNA
using Microsoft.Xna.Framework.Graphics;
#endif

namespace Daramkun.Liqueur.Graphics
{
	public class Image : IImage
	{
#if OPENTK
		internal int texture;
#elif XNA
		Texture2D texture;
#endif
		Vector2 textureSize;

		public int Width { get { return ( int ) textureSize.X; } }
		public int Height { get { return ( int ) textureSize.Y; } }

		public Vector2 Size { get { return textureSize; } }

		public Image ( Stream stream )
		{
			ImageData? imageData = ImageDecoders.GetImageData ( stream );
			InitializeBitmap ( imageData.Value.ImageDecoder.GetPixels ( imageData.Value, Color.Transparent ),
				imageData.Value.Width, imageData.Value.Height );
		}

		public Image ( Stream stream, Color colorKey )
		{
			ImageData? imageData = ImageDecoders.GetImageData ( stream );
			InitializeBitmap ( imageData.Value.ImageDecoder.GetPixels ( imageData.Value, colorKey ),
				imageData.Value.Width, imageData.Value.Height );
		}

		public Image ( ImageData imageData )
		{
			InitializeBitmap ( imageData.ImageDecoder.GetPixels ( imageData, Color.Transparent ),
				imageData.Width, imageData.Height );
		}

		public Image ( ImageData imageData, Color colorKey )
		{
			InitializeBitmap ( imageData.ImageDecoder.GetPixels ( imageData, colorKey ),
				imageData.Width, imageData.Height );
		}

		~Image ()
		{
			Dispose ( false );
		}

		private void InitializeBitmap ( Color [] pixels, int width, int height )
		{
			textureSize = new Vector2 ( width, height );

#if OPENTK
			texture = GL.GenTexture ();
			GL.BindTexture ( TextureTarget.Texture2D, texture );

			uint magFilter = ( uint ) TextureMagFilter.Linear;
			GL.TexParameterI ( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ref magFilter );
			uint minFilter = ( uint ) TextureMinFilter.Linear;
			GL.TexParameterI ( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ref minFilter );
			uint wrapS = ( uint ) TextureWrapMode.ClampToEdge;
			GL.TexParameterI ( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ref wrapS );
			uint wrapT = ( uint ) TextureWrapMode.ClampToEdge;
			GL.TexParameterI ( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ref wrapT );
			
			byte [] colorData = new byte [ width * height * 4 ];
			
			for ( int i = 0, index = 0; i < pixels.Length; i++ )
			{
				colorData [ index++ ] = pixels [ i ].BlueValue;
				colorData [ index++ ] = pixels [ i ].GreenValue;
				colorData [ index++ ] = pixels [ i ].RedValue;
				colorData [ index++ ] = pixels [ i ].AlphaValue;
			}

			GL.TexImage2D<byte> ( TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba8,
				width, height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra,
				PixelType.UnsignedByte, colorData );

			GL.BindTexture ( TextureTarget.Texture2D, 0 );
#elif XNA
			texture = new Texture2D ( ( LiqueurSystem.Renderer as Renderer ).GraphicsDevice, width, height );
#endif
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
#if OPENTK
				GL.DeleteTexture ( texture );
				texture = 0;
#elif XNA
				texture.Dispose ();
				texture = null;
#endif
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( true );
		}

		public void DrawBitmap ( Color overlay, Transform2 transform )
		{
			DrawBitmap ( overlay, transform, new Rectangle ( Vector2.Zero, Size ) );
		}

		public void DrawBitmap ( Color overlay, Transform2 transform, Rectangle sourceRectangle )
		{
#if OPENTK
			GL.BindTexture ( TextureTarget.Texture2D, texture );

			GL.TexCoordPointer ( 2, TexCoordPointerType.Float, 0, new float [] {
					sourceRectangle.Position.X / Size.X, sourceRectangle.Position.Y / Size.Y,
					(sourceRectangle.Position.X + sourceRectangle.Size.X) / Size.X, sourceRectangle.Position.X / Size.Y,
					sourceRectangle.Position.X / Size.X, (sourceRectangle.Position.Y + sourceRectangle.Size.Y) / Size.Y,
					(sourceRectangle.Position.X + sourceRectangle.Size.X) / Size.X, (sourceRectangle.Position.Y + sourceRectangle.Size.Y) / Size.Y
				} );

			GL.VertexPointer ( 2, VertexPointerType.Float, 0, new float [] { 
					0, 0, 
					sourceRectangle.Size.X, 0, 
					0, sourceRectangle.Size.Y, 
					sourceRectangle.Size.X, sourceRectangle.Size.Y
				} );

			GL.PushMatrix ();

			GL.Translate ( transform.Translate.X + transform.RotationCenter.X, transform.Translate.Y + transform.RotationCenter.Y, 0 );
			GL.Rotate ( transform.Rotation, 0, 0, 1 );
			GL.Translate ( -transform.RotationCenter.X + transform.ScaleCenter.X, -transform.RotationCenter.Y + transform.ScaleCenter.Y, 0 );
			GL.Scale ( transform.Scale.X, transform.Scale.Y, 1 );
			GL.Translate ( -transform.ScaleCenter.X, -transform.ScaleCenter.Y, 0 );

			GL.Color4 ( overlay.RedScalar, overlay.GreenScalar, overlay.BlueScalar, overlay.AlphaScalar );
			try
			{
				GL.DrawArrays ( BeginMode.TriangleStrip, 0, 4 );
			}
			catch ( AccessViolationException ex ) { Debug.WriteLine ( ex ); }
			GL.PopMatrix ();
#elif XNA
			SpriteBatch spriteBatch = ( LiqueurSystem.Renderer as Renderer ).SpriteBatch;
			spriteBatch.Draw ( 
				texture,
				new Microsoft.Xna.Framework.Vector2 ( transform.Translate.X, transform.Translate.Y ),
				new Microsoft.Xna.Framework.Rectangle ( ( int ) sourceRectangle.Position.X, ( int ) sourceRectangle.Position.Y,
					( int ) sourceRectangle.Size.X, ( int ) sourceRectangle.Size.Y ),
				new Microsoft.Xna.Framework.Color (
				overlay.RedScalar, overlay.GreenScalar, overlay.BlueScalar, overlay.AlphaScalar ),
				transform.Rotation,
				new Microsoft.Xna.Framework.Vector2 ( transform.RotationCenter.X, transform.RotationCenter.Y ),
				new Microsoft.Xna.Framework.Vector2 ( transform.Scale.X, transform.Scale.Y ),
				SpriteEffects.None,
				0
				);
#endif
		}
	}
}
