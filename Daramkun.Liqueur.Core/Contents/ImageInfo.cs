using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.Decoder.Images;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Contents
{
	/// <summary>
	/// Image information structure
	/// </summary>
	public struct ImageInfo
	{
		/// <summary>
		/// Image Width
		/// </summary>
		public int Width { get; set; }
		/// <summary>
		/// Image Height
		/// </summary>
		public int Height { get; set; }
		/// <summary>
		/// Pixel Data (Undecoded data)
		/// </summary>
		public object Data { get; set; }
		/// <summary>
		/// Image Stream
		/// </summary>
		public Stream ImageStream { get; set; }
		/// <summary>
		/// Image Decoder
		/// </summary>
		public IImageDecoder ImageDecoder { get; set; }

		/// <summary>
		/// Get image pixels
		/// </summary>
		/// <param name="colorKey">Color key (if you need)</param>
		/// <returns>Image pixels</returns>
		public Color [] GetPixel ( Color? colorKey ) { return ImageDecoder.GetPixel ( this, colorKey ); }

		/// <summary>
		/// Image informations string
		/// </summary>
		/// <returns></returns>
		public override string ToString ()
		{
			return string.Format ( "{{Width:{0}, Height:{1}}}", Width, Height );
		}
	}
}
