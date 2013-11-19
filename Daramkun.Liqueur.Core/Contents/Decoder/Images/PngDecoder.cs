using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Contents.Decoder.Images
{
	/// <summary>
	/// Portable network graphics image decoder
	/// </summary>
	[FileFormat ( "PNG" )]
	public class PngDecoder : IImageDecoder
	{
		/// <summary>
		/// PNG image decode
		/// </summary>
		/// <param name="stream">PNG file</param>
		/// <param name="args">argument, don't set this</param>
		/// <returns>Image information and pixel data</returns>
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

		/// <summary>
		/// Get PNG pixels
		/// </summary>
		/// <param name="imageInfo">Image information</param>
		/// <param name="colorKey">Color key (if you need)</param>
		/// <returns>Image pixels</returns>
		public Color [] GetPixel ( ImageInfo imageInfo, Color? colorKey )
		{
			Hjg.Pngcs.PngReader reader = ( ( object ) imageInfo.Data ) as Hjg.Pngcs.PngReader;
			MemoryStream stream = new MemoryStream ();
			for ( int i = 0; i < imageInfo.Height; ++i )
			{
				Hjg.Pngcs.ImageLine column = reader.ReadRowByte ( i );
				byte [] data = column.GetScanlineByte ();
				stream.Write ( data, 0, data.Length );
			}
			
			byte [] pixels = stream.ToArray ();
			Color [] colors = new Color [ imageInfo.Width * imageInfo.Height ];
			for ( int i = 0, index = 0; i < pixels.Length; i += reader.ImgInfo.BytesPixel, ++index )
			{
				if ( reader.ImgInfo.BytesPixel == 3 )
					colors [ index ] = new Color ( pixels [ i + 0 ], pixels [ i + 1 ], pixels [ i + 2 ] );
				else if ( reader.ImgInfo.BytesPixel == 1 )
					colors [ index ] = new Color ( pixels [ i ], pixels [ i ], pixels [ i ], 255 );
				else
					colors [ index ] = new Color ( pixels [ i + 0 ], pixels [ i + 1 ], pixels [ i + 2 ], pixels [ i + 3 ] );
			}
			return colors;
		}

		/// <summary>
		/// Decoder information string
		/// </summary>
		/// <returns></returns>
		public override string ToString ()
		{
			return "Portable Network Graphics(PNG) Decoder by PngCs";
		}
	}
}
