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
	/// <summary>
	/// Ogg vorbis audio Decoder
	/// </summary>
	[FileFormat ( "OGG" )]
	public class OggVorbisDecoder : IAudioDecoder
	{
		/// <summary>
		/// OGG audio decode
		/// </summary>
		/// <param name="stream">OGG file</param>
		/// <param name="args">argument, don't set this</param>
		/// <returns>Audio information and PCM data</returns>
		public AudioInfo Decode ( Stream stream, params object [] args )
		{
			try
			{
				OggDecodeStream decodeStream = new OggDecodeStream ( stream );
				return new WaveDecoder().Decode ( decodeStream );
			}
			catch { throw new FileFormatMismatchException (); }
		}

		/// <summary>
		/// Get OGG samples
		/// </summary>
		/// <param name="audioInfo">Audio information</param>
		/// <param name="timeSpan">Audio position (if you need)</param>
		/// <returns>Audio sample</returns>
		public byte [] GetSample ( AudioInfo audioInfo, TimeSpan? timeSpan )
		{
			return audioInfo.GetSample ( timeSpan );
		}

		/// <summary>
		/// Decoder information string
		/// </summary>
		/// <returns></returns>
		public override string ToString ()
		{
			return "OGG Vorbis Decoder by csOgg/csVorbis";
		}
	}
}
