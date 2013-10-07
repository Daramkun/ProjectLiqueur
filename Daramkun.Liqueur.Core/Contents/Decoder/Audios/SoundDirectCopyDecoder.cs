using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Exceptions;

namespace Daramkun.Liqueur.Contents.Decoder.Audios
{
	public class SoundDirectCopyDecoder : IAudioDecoder
	{
		byte [] data;

		public SoundDirectCopyDecoder ( byte [] data )
		{
			this.data = data;
		}

		public AudioInfo Decode ( Stream stream, params object [] args )
		{
			throw new FileFormatMismatchException ();
		}

		public byte [] GetSample ( AudioInfo audioInfo, TimeSpan? timeSpan )
		{
			return data;
		}
	}
}
