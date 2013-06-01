using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Decoders.Sounds.OggVorbisCore;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.Medias;

namespace Daramkun.Liqueur.Decoders.Sounds
{
	[FileFormat ( "OGG" )]
	public class DefaultOggVobisDecoder : ISoundDecoder
	{
		public SoundData? Decode ( Stream stream )
		{
			try
			{
				OggDecodeStream decodeStream = new OggDecodeStream ( stream );
				return SoundDecoders.GetSoundData ( SoundDecoders.GetSoundDecoder ( "WAV" ),
					decodeStream );
			}
			catch { throw new FileFormatMismatchException (); }
		}

		public byte [] GetSample ( SoundData soundData )
		{
			return SoundDecoders.GetSoundDecoder ( "WAV" ).GetSample ( soundData );
		}

		public override string ToString ()
		{
			return "OGG Vorbis Decoder";
		}
	}
}
