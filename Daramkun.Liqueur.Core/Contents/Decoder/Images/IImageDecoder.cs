using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Contents.Decoder.Images
{
	/// <summary>
	/// Bitmap decoder interface
	/// </summary>
	public interface IImageDecoder : IDecoder<ImageInfo>
	{
		/// <summary>
		/// Get image pixels
		/// </summary>
		/// <param name="info">Bitmap information</param>
		/// <param name="colorKey">Color key (if you need)</param>
		/// <returns>Image pixels</returns>
		Color [] GetPixel ( ImageInfo info, Color? colorKey );
	}
}
