using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Decoders;
using Daramkun.Liqueur.Decoders.Images;
using Daramkun.Liqueur.Math;

namespace Daramkun.Liqueur.Graphics
{
	class Image : IImage
	{
		SharpDX.Direct2D1.Bitmap texture;

		private void InitializeBitmap ( Color [] pixels, int width, int height )
		{
			textureSize = new Vector2 ( width, height );
			SharpDX.Direct2D1.DeviceContext context = ( LiqueurSystem.Renderer as Renderer ).d2dDeviceContext;
			texture = SharpDX.Direct2D1.Bitmap.New<Color> ( context,
				new SharpDX.DrawingSize ( width, height ), pixels,
				new SharpDX.Direct2D1.BitmapProperties ()
				{
					DpiX = ( LiqueurSystem.Renderer as Renderer ).d2dDeviceContext.DotsPerInch.Width,
					DpiY = ( LiqueurSystem.Renderer as Renderer ).d2dDeviceContext.DotsPerInch.Height,
					PixelFormat = new SharpDX.Direct2D1.PixelFormat ( SharpDX.DXGI.Format.B8G8R8A8_UNorm, SharpDX.Direct2D1.AlphaMode.Premultiplied )
				} );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				texture.Dispose ();
				texture = null;
			}
		}

		public void DrawBitmap ( Color overlay, Transform2 transform, Rectangle sourceRectangle )
		{
			SharpDX.Direct2D1.DeviceContext context = ( LiqueurSystem.Renderer as Renderer ).d2dDeviceContext;

			SharpDX.Matrix3x2 matrix = SharpDX.Matrix3x2.Translation ( transform.Translate.X + transform.RotationCenter.X,
				transform.Translate.Y + transform.RotationCenter.Y );
			matrix *= SharpDX.Matrix3x2.Rotation ( transform.Rotation );
			matrix *= SharpDX.Matrix3x2.Translation ( -transform.RotationCenter.X + transform.ScaleCenter.X,
				-transform.RotationCenter.Y + transform.ScaleCenter.Y );
			matrix *= SharpDX.Matrix3x2.Scaling ( transform.Scale.X, transform.Scale.Y );
			matrix *= SharpDX.Matrix3x2.Translation ( -transform.ScaleCenter.X, -transform.ScaleCenter.Y );

			SharpDX.Matrix3x2 oldMatrix = context.Transform;
			context.Transform = matrix;

			SharpDX.RectangleF innerSourceRectangle = new SharpDX.RectangleF ( sourceRectangle.Position.X, sourceRectangle.Position.Y,
					sourceRectangle.Size.X, sourceRectangle.Size.Y );
			context.DrawBitmap ( texture, overlay.AlphaScalar, SharpDX.Direct2D1.BitmapInterpolationMode.Linear,
				innerSourceRectangle );

			context.Transform = oldMatrix;
		}
			
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

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( true );
		}

		public void DrawBitmap ( Color overlay, Transform2 transform )
		{
			DrawBitmap ( overlay, transform, new Rectangle ( Vector2.Zero, Size ) );
		}
	}
}
