using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.Decoder.Images;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Contents
{
	public struct ImageInfo
	{
		public int Width { get; set; }
		public int Height { get; set; }
		public object Data { get; set; }
		public Stream ImageStream { get; set; }
		public IImageDecoder ImageDecoder { get; set; }

		public Color [] GetPixel ( Color? colorKey ) { return ImageDecoder.GetPixel ( this, colorKey ); }

		public override string ToString ()
		{
			return string.Format ( "{{Width:{0}, Height:{1}}}", Width, Height );
		}
	}
}
