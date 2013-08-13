using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Common;
using SharpDX;
using SharpDX.MediaFoundation;

namespace Daramkun.Liqueur.Contents.Decoder.Audios
{
	[FileFormat("WAV","MP3","WMA")]
	public class MFAudioDecoder : IAudioDecoder
	{
		private class SampleStruct
		{
			public AudioDecoder AudioDecoder { get; set; }
			public IEnumerator<DataPointer> DataPointers { get; set; }
		}

		public AudioInfo Decode ( Stream stream, params object [] args )
		{
			AudioDecoder decoder = new AudioDecoder ( stream );

			AudioInfo audioInfo = new AudioInfo ();
			audioInfo.AudioChannel = ( AudioChannel ) decoder.WaveFormat.Channels;
			audioInfo.BitPerSample = decoder.WaveFormat.BitsPerSample / 8;
			audioInfo.SampleRate = decoder.WaveFormat.SampleRate;

			audioInfo.AudioStream = stream;
			audioInfo.AudioDecoder = this;

			audioInfo.Samples = new SampleStruct () { AudioDecoder = decoder, DataPointers = decoder.GetSamples ().GetEnumerator () };

			return audioInfo;
		}

		public byte [] GetSample ( AudioInfo audioInfo, TimeSpan? timeSpan )
		{
			SampleStruct samples = audioInfo.Samples as SampleStruct;
			if ( timeSpan != null )
				samples.DataPointers = samples.AudioDecoder.GetSamples ( timeSpan.Value ).GetEnumerator ();
			if ( !samples.DataPointers.MoveNext () ) return null;
			DataPointer data = samples.DataPointers.Current;
			DataStream stream = new DataStream ( data.Pointer, data.Size, true, false );
			byte [] buffer = new byte [ stream.Length ];
			stream.Read ( buffer, 0, ( int ) stream.Length );
			return buffer;
		}
	}
}
