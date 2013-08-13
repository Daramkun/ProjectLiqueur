using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.Decoder.Videos;

namespace Daramkun.Liqueur.Contents
{
	public struct VideoInfo
	{
		/// <summary>
		/// Total length of video
		/// </summary>
		public TimeSpan Duration { get; set; }
		/// <summary>
		/// Video width
		/// </summary>
		public int Width { get; set; }
		/// <summary>
		/// Video height
		/// </summary>
		public int Height { get; set; }

		/// <summary>
		/// Video stream
		/// </summary>
		public Stream VideoStream { get; set; }
		/// <summary>
		/// Video decoder
		/// </summary>
		public IVideoDecoder VideoDecoder { get; set; }

		public AudioInfo GetAudio ()
		{
			return VideoDecoder.GetAudio ( this );
		}

		public ImageInfo GetImage ( TimeSpan? timeSpan )
		{
			return VideoDecoder.GetImage ( this, timeSpan );
		}
	}
}
