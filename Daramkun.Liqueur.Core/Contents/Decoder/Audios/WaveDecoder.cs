using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Exceptions;

namespace Daramkun.Liqueur.Contents.Decoder.Audios
{
	[FileFormat ( "WAV" )]
	public class WaveDecoder : IAudioDecoder
	{
		private bool ReadRIFFHeader ( BinaryReader br, out int fileSize )
		{
			string riffSignature = Encoding.UTF8.GetString ( br.ReadBytes ( 4 ), 0, 4 );
			if ( riffSignature == "WAVE" ) { fileSize = 0; return true; }

			fileSize = br.ReadInt32 ();
			string waveSignature = Encoding.UTF8.GetString ( br.ReadBytes ( 4 ), 0, 4 );

			if ( riffSignature != "RIFF" || waveSignature != "WAVE" )
				return false;

			return true;
		}

		private bool ReadfmtHeader ( BinaryReader br, ref AudioInfo audioInfo, out int byteRate )
		{
			string fmtSignature = Encoding.UTF8.GetString ( br.ReadBytes ( 4 ), 0, 4 ).Trim ();
			if ( fmtSignature != "fmt" ) { byteRate = 0; return false; }

			int chunkSize = br.ReadInt32 ();
			int audioFormat = br.ReadInt16 ();
			if ( audioFormat != 1 ) { byteRate = 0; return false; }
			audioInfo.AudioChannel = ( AudioChannel ) br.ReadInt16 ();
			audioInfo.SampleRate = br.ReadInt32 ();
			byteRate = br.ReadInt32 ();

			int blockAlign = br.ReadInt16 ();
			audioInfo.BitPerSample = br.ReadInt16 () / 8;

			if ( chunkSize != 16 )
			{
				int extraSize = br.ReadInt16 ();
				br.ReadBytes ( extraSize );
			}

			return true;
		}

		private class SampleInfo
		{
			public int StartPoint { get; set; }
			public int DataSize { get; set; }
			public int Offset { get; set; }
			public int ByteRate { get; set; }
			public BinaryReader Reader { get; set; }
		}

		private bool ReadWaveChunk ( BinaryReader br, ref AudioInfo audioInfo, out int dataSize )
		{
			string fmtSignature = Encoding.UTF8.GetString ( br.ReadBytes ( 4 ), 0, 4 );
			if ( fmtSignature != "data" )
			{
				if ( fmtSignature == "fact" )
				{
					br.ReadInt32 ();
					br.ReadInt32 ();
					return ReadWaveChunk ( br, ref audioInfo, out dataSize );
				}
				else
				{
					dataSize = 0;
					return false;
				}
			}

			dataSize = br.ReadInt32 ();
			audioInfo.Samples = new SampleInfo ()
			{
				DataSize = dataSize, 
				Offset = 0, 
				Reader = br, 
				StartPoint = ( int ) br.BaseStream.Position
			};

			return true;
		}

		public AudioInfo Decode ( Stream stream, params object [] args )
		{
			AudioInfo audioInfo = new AudioInfo ();
			BinaryReader br = new BinaryReader ( stream );
			int chunkSize;
			if ( !ReadRIFFHeader ( br, out chunkSize ) )
				throw new FileFormatMismatchException ();

			int byteRate, dataSize;
			ReadfmtHeader ( br, ref audioInfo, out byteRate );
			ReadWaveChunk ( br, ref audioInfo, out dataSize );

			( audioInfo.Samples as SampleInfo ).ByteRate = byteRate;
			audioInfo.Duration = TimeSpan.FromSeconds ( dataSize / ( float ) byteRate );

			audioInfo.AudioStream = stream;
			audioInfo.AudioDecoder = this;

			return audioInfo;
		}

		public byte [] GetSample ( AudioInfo audioInfo, TimeSpan? timeSpan )
		{
			SampleInfo sampleInfo = audioInfo.Samples as SampleInfo;

			if ( timeSpan != null )
				audioInfo.AudioStream.Position = sampleInfo.StartPoint + ( int ) ( timeSpan.Value.TotalSeconds * sampleInfo.ByteRate );

			if ( ( int ) sampleInfo.DataSize <= ( int ) sampleInfo.Offset )
			{
				audioInfo.AudioStream.Dispose ();
				return null;
			}
			byte [] data = sampleInfo.Reader.ReadBytes ( audioInfo.SampleRate );
			sampleInfo.Offset += audioInfo.SampleRate;
			return data;
		}

		public override string ToString ()
		{
			return "Wave Decoder";
		}
	}
}
