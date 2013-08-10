using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Contents.Decoder.Images
{
	public class PngDecoder : IImageDecoder
	{
		public ImageInfo Decode ( Stream stream, params object [] args )
		{
			try
			{
				Hjg.Pngcs.PngReader pngReader = new Hjg.Pngcs.PngReader ( stream );
				Hjg.Pngcs.ImageInfo imgInfo = pngReader.ImgInfo;

				ImageInfo imageInfo = new ImageInfo ();
				imageInfo.Width = imgInfo.Cols;
				imageInfo.Height = imgInfo.Rows;
				imageInfo.Data = pngReader;

				imageInfo.ImageStream = stream;
				imageInfo.ImageDecoder = this;
				return imageInfo;
			}
			catch { throw new FileFormatMismatchException (); }
		}

		public Color [] GetPixel ( ImageInfo imageInfo, Color? colorKey )
		{
			Hjg.Pngcs.PngReader reader = ( ( object ) imageInfo.Data ) as Hjg.Pngcs.PngReader;
			MemoryStream stream = new MemoryStream ();
			for ( int i = 0; i < imageInfo.Height; i++ )
			{
				Hjg.Pngcs.ImageLine column = reader.ReadRowByte ( i );
				byte [] data = column.GetScanlineByte ();
				stream.Write ( data, 0, data.Length );
			}

			byte [] pixels = stream.ToArray ();
			Color [] colors = new Color [ imageInfo.Width * imageInfo.Height ];
			for ( int i = 0, index = 0; i < pixels.Length; i += ( pixels.Length % 3 == 0 ) ? 3 : 4, index++ )
			{
				if ( pixels.Length % 3 == 0 )
					colors [ index ] = new Color ( pixels [ i + 0 ], pixels [ i + 1 ], pixels [ i + 2 ] );
				else
					colors [ index ] = new Color ( pixels [ i + 0 ], pixels [ i + 1 ], pixels [ i + 2 ], pixels [ i + 3 ] );
			}
			return colors;
		}

		public override string ToString ()
		{
			return "Portable Network Graphics(PNG) Decoder by PngCs";
		}
	}
}
