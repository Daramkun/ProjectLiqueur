using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Contents.Decoder.Audios;

namespace Daramkun.Liqueur.Contents
{
	public struct AudioInfo
	{
		public AudioChannel AudioChannel { get; set; }
		public int SampleRate { get; set; }
		public int BitPerSample { get; set; }
		public object Samples { get; set; }
		public TimeSpan Duration { get; set; }
		public Stream AudioStream { get; internal set; }
		public IAudioDecoder AudioDecoder { get; set; }

		public byte [] GetSample ( TimeSpan? timeSpan )
		{
			return AudioDecoder.GetSample ( this, timeSpan );
		}

		public override string ToString ()
		{
			return String.Format ( "{{Audio Channel:{0}, Sample Rate:{1}, Bit per Sample:{2}, Duration:{3}}}",
				AudioChannel, SampleRate, BitPerSample, Duration );
		}
	}
}
