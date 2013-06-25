using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Decoders;

namespace Daramkun.Liqueur.Medias
{
	/// <summary>
	/// 사운드 데이터
	/// </summary>
	public struct SoundData
	{
		public AudioChannel AudioChannel { get; set; }
		public int SampleRate { get; set; }
		public BitPerSample BitPerSample { get; set; }
		public object Samples { get; set; }
		public TimeSpan Duration { get; set; }
		public Stream SoundStream { get; internal set; }
		public ISoundDecoder SoundDecoder { get; set; }

		public override string ToString ()
		{
			return String.Format ( "{{Audio Channel:{0}, Sample Rate:{1}, Bit per Sample:{2}, Duration:{3}}}",
				AudioChannel, SampleRate, BitPerSample, Duration );
		}
	}
}
