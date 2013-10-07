using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Contents.Decoder.Images
{
	public class ImageDirectCopyDecoder : IImageDecoder
	{
		Color [] data;

		public ImageDirectCopyDecoder ( Color [] data )
		{
			this.data = data;
		}

		public ImageInfo Decode ( Stream stream, params object [] args )
		{
			throw new FileFormatMismatchException ();
		}

		public Color [] GetPixel ( ImageInfo info, Color? colorKey )
		{
			return data;
		}
	}
}
