using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Decoders.Sounds.OggVorbisCore;
using Daramkun.Liqueur.Exceptions;

namespace Daramkun.Liqueur.Contents.Decoder.Audios
{
	[FileFormat ( "OGG" )]
	public class OggVorbisDecoder : IAudioDecoder
	{
		public AudioInfo Decode ( Stream stream, params object [] args )
		{
			try
			{
				OggDecodeStream decodeStream = new OggDecodeStream ( stream );
				return new WaveDecoder().Decode ( decodeStream );
			}
			catch { throw new FileFormatMismatchException (); }
		}

		public byte [] GetSample ( AudioInfo audioInfo, TimeSpan? timeSpan )
		{
			return audioInfo.GetSample ( timeSpan );
		}

		public override string ToString ()
		{
			return "OGG Vorbis Decoder by csOgg/csVorbis";
		}
	}
}
