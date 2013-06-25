using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Decoders.Images
{
	[FileFormat ( "DIRECTDATA" )]
	public class DirectPixelDecoder : IImageDecoder
	{
		[Obsolete ( "This method is not support in DirectPixelDecoder", true )]
		public Graphics.ImageData? Decode ( System.IO.Stream stream )
		{
			throw new NotImplementedException ();
		}

		public Color [] GetPixels ( ImageData imageData, Color colorKey )
		{
			object data = imageData.Data;
			return data as Color [];
		}
	}
}
