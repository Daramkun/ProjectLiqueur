using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.Medias;

namespace Daramkun.Liqueur.Decoders.Sounds
{
	[FileFormat ( "WAV" )]
	public class DefaultWaveDecoder : ISoundDecoder
	{
		private bool ReadRIFFHeader ( BinaryReader br, out int fileSize )
		{
			string riffSignature = Encoding.UTF8.GetString ( br.ReadBytes ( 4 ), 0, 4 );
			fileSize = br.ReadInt32 ();
			string waveSignature = Encoding.UTF8.GetString ( br.ReadBytes ( 4 ), 0, 4 );

			if ( riffSignature != "RIFF" || waveSignature != "WAVE" )
				return false;

			return true;
		}

		private bool ReadfmtHeader ( BinaryReader br, ref SoundData soundData, out int byteRate )
		{
			string fmtSignature = Encoding.UTF8.GetString ( br.ReadBytes ( 4 ), 0, 4 ).Trim ();
			if ( fmtSignature != "fmt" ) { byteRate = 0; return false; }

			int chunkSize = br.ReadInt32 ();
			int audioFormat = br.ReadInt16 ();
			if ( audioFormat != 1 ) { byteRate = 0; return false; }
			soundData.AudioChannel = ( AudioChannel ) br.ReadInt16 ();
			soundData.SampleRate = br.ReadInt32 ();
			byteRate = br.ReadInt32 ();

			int blockAlign = br.ReadInt16 ();
			soundData.BitPerSample = ( BitPerSample ) ( br.ReadInt16 () / 8 );

			if ( chunkSize != 16 )
			{
				int extraSize = br.ReadInt16 ();
				br.ReadBytes ( extraSize );
			}

			return true;
		}

		private bool ReadWaveChunk ( BinaryReader br, ref SoundData soundData, out int dataSize )
		{
			string fmtSignature = Encoding.UTF8.GetString ( br.ReadBytes ( 4 ), 0, 4 );
			if ( fmtSignature != "data" )
			{
				if ( fmtSignature == "fact" )
				{
					br.ReadInt32 ();
					br.ReadInt32 ();
					return ReadWaveChunk ( br, ref soundData, out dataSize );
				}
				else
				{
					dataSize = 0;
					return false;
				}
			}

			dataSize = br.ReadInt32 ();
			soundData.Samples = new object [] { dataSize, 0, br };

			return true;
		}

		public SoundData? Decode ( Stream stream )
		{
			SoundData soundData = new SoundData ();
			BinaryReader br = new BinaryReader ( stream );
			int chunkSize;
			if ( !ReadRIFFHeader ( br, out chunkSize ) )
				throw new FileFormatMismatchException ();

			int byteRate, dataSize;
			ReadfmtHeader ( br, ref soundData, out byteRate );
			ReadWaveChunk ( br, ref soundData, out dataSize );

			soundData.Duration = TimeSpan.FromSeconds ( dataSize / ( float ) byteRate );

			return soundData;
		}

		public byte [] GetSample ( SoundData soundData )
		{
			object [] obj = soundData.Samples as object [];
			if ( ( int ) obj [ 0 ] <= ( int ) obj [ 1 ] )
			{
				soundData.SoundStream.Dispose ();
				return null;
			}
			byte [] data = ( obj [ 2 ] as BinaryReader ).ReadBytes ( soundData.SampleRate );
			obj [ 1 ] = ( ( int ) obj [ 1 ] + soundData.SampleRate );
			return data;
		}

		public override string ToString ()
		{
			return "Wave Decoder";
		}
	}
}