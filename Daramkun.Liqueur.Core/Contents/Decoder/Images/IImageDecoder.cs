using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Contents.Decoder.Images
{
	public interface IImageDecoder : IDecoder<ImageInfo>
	{
		Color [] GetPixel ( ImageInfo info, Color? colorKey );
	}
}
