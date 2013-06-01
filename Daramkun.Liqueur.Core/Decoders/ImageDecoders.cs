using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Decoders.Images;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Decoders
{
	public static class ImageDecoders
	{
		static List<IImageDecoder> imageDecoders;

		public static IEnumerable<IImageDecoder> Decoders { get { return imageDecoders; } }

		static ImageDecoders ()
		{
			imageDecoders = new List<IImageDecoder> ();
		}

		public static void AddDecoder ( Type decoderType )
		{
			imageDecoders.Add ( Activator.CreateInstance ( decoderType ) as IImageDecoder );
		}

		public static IImageDecoder GetImageDecoder ( string fileFormat )
		{
			foreach ( IImageDecoder imageDecoder in imageDecoders )
			{
				var attrs = imageDecoder.GetType ().GetCustomAttributes ( typeof ( FileFormatAttribute ), true );
				if ( attrs.Length == 0 ) return null;
				foreach ( string fileExtension in ( attrs [ 0 ] as FileFormatAttribute ).FileExtension )
					if ( fileExtension == fileFormat.ToUpper ().Trim () )
						return imageDecoder;
			}

			return null;
		}

		public static ImageData? GetImageData ( Stream stream )
		{
			foreach ( IImageDecoder imageDecoder in ImageDecoders.Decoders )
			{
				try
				{
					stream.Position = 0;
					ImageData? imageData = imageDecoder.Decode ( stream );
					if ( imageData != null )
					{
						ImageData returnData = imageData.Value;
						returnData.ImageDecoder = imageDecoder;
						returnData.ImageStream = stream;
						return returnData;
					}
					else return null;
				}
				catch ( FileFormatMismatchException fileFormatMismatchException )
				{
					fileFormatMismatchException.ToString ();
					continue;
				}
				catch { return null; }
			}
			return null;
		}

		public static ImageData? GetImageData<T> ( Stream stream ) where T : IImageDecoder
		{
			try
			{
				IImageDecoder decoder = Activator.CreateInstance<T> ();
				return decoder.Decode ( stream );
			}
			catch { return null; }
		}

		public static ImageData? GetImageData ( IImageDecoder imageDecoder, Stream stream )
		{
			try
			{
				return imageDecoder.Decode ( stream );
			}
			catch { return null; }
		}

		public static void AddDefaultDecoders ()
		{
			AddDecoder ( typeof ( DefaultBitmapDecoder ) );
			AddDecoder ( typeof ( DirectPixelDecoder ) );
		}
	}
}
