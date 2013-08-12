using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Contents.Decoder.Audios;

namespace Daramkun.Liqueur.Contents
{
	/// <summary>
	/// Audio Information structure
	/// </summary>
	public struct AudioInfo
	{
		/// <summary>
		/// Audio Channel
		/// </summary>
		public AudioChannel AudioChannel { get; set; }
		/// <summary>
		/// Audio samplerate
		/// </summary>
		public int SampleRate { get; set; }
		/// <summary>
		/// Bit per sample
		/// </summary>
		public int BitPerSample { get; set; }
		/// <summary>
		/// Samples (Undecoded data)
		/// </summary>
		public object Samples { get; set; }
		/// <summary>
		/// Total length of audio
		/// </summary>
		public TimeSpan Duration { get; set; }
		/// <summary>
		/// Audio stream
		/// </summary>
		public Stream AudioStream { get; set; }
		/// <summary>
		/// Audio Decoder
		/// </summary>
		public IAudioDecoder AudioDecoder { get; set; }

		/// <summary>
		/// Get Samples
		/// </summary>
		/// <param name="timeSpan">Audio position (if you need)</param>
		/// <returns>Audio sample</returns>
		public byte [] GetSample ( TimeSpan? timeSpan )
		{
			return AudioDecoder.GetSample ( this, timeSpan );
		}

		/// <summary>
		/// Audio Informations string
		/// </summary>
		/// <returns></returns>
		public override string ToString ()
		{
			return String.Format ( "{{Audio Channel:{0}, Sample Rate:{1}, Bit per Sample:{2}, Duration:{3}}}",
				AudioChannel, SampleRate, BitPerSample, Duration );
		}
	}
}
