using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Decoders;
using Daramkun.Liqueur.Decoders.Images;
using Daramkun.Liqueur.Geometries;

namespace Daramkun.Liqueur.Graphics
{
	public class Image : IImage
	{
		SharpDX.Direct2D1.Bitmap texture;
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
			texture = new SharpDX.Direct2D1.Bitmap ( ( LiqueurSystem.Renderer as Renderer ).d2dDeviceContext, new SharpDX.DrawingSize ( width, height ) );
			texture.CopyFromMemory<Color> ( pixels );
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
			GC.SuppressFinalize ( true );
		}

		public void DrawBitmap ( Color overlay, Transform2 transform )
		{
			DrawBitmap ( overlay, transform, new Rectangle ( Vector2.Zero, Size ) );
		}

		public void DrawBitmap ( Color overlay, Transform2 transform, Rectangle sourceRectangle )
		{
			SharpDX.Direct2D1.DeviceContext context = ( LiqueurSystem.Renderer as Renderer ).d2dDeviceContext;
			
			context.DrawBitmap ( texture, 1, SharpDX.Direct2D1.BitmapInterpolationMode.Linear );
		}
	}
}
