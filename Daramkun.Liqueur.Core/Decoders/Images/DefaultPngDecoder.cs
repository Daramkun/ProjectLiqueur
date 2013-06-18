using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Decoders.Images
{
	public class DefaultPngDecoder : IImageDecoder
	{
		public ImageData? Decode ( Stream stream )
		{
			try
			{
				Hjg.Pngcs.PngReader pngReader = new Hjg.Pngcs.PngReader ( stream );
				Hjg.Pngcs.ImageInfo imageInfo = pngReader.ImgInfo;

				ImageData imageData = new ImageData ();
				imageData.Width = imageInfo.Cols;
				imageData.Height = imageInfo.Rows;
				imageData.Data = pngReader;

				return imageData;
			}
			catch { throw new FileFormatMismatchException (); }
		}

		public Color [] GetPixels ( ImageData imageData, Color colorKey )
		{
			Hjg.Pngcs.PngReader reader = ( ( object ) imageData.Data ) as Hjg.Pngcs.PngReader;
			MemoryStream stream = new MemoryStream ();
			for ( int i = 0; i < imageData.Height; i++ )
			{
				Hjg.Pngcs.ImageLine column = reader.ReadRowByte ( i );
				byte[] data = column.GetScanlineByte ();
				stream.Write ( data, 0, data.Length );
			}

			byte [] pixels = stream.ToArray ();
			Color [] colors = new Color [ imageData.Width * imageData.Height ];
			for ( int i = 0, index = 0; i < pixels.Length; i += ( pixels.Length % 3 == 0 ) ? 3 : 4, index++ )
			{
				if ( pixels.Length % 3 == 0 )
					colors [ index ] = new Color ( pixels [ i + 0 ], pixels [ i + 1 ], pixels [ i + 2 ] );
				else
					colors [ index ] = new Color ( pixels [ i + 0 ], pixels [ i + 1 ], pixels [ i + 2 ], pixels [ i + 3 ] );
			}
			return colors;
		}
	}
}
