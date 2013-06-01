using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Decoders.Sounds;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.Medias;

namespace Daramkun.Liqueur.Decoders
{
	public static class SoundDecoders
	{
		static List<ISoundDecoder> soundDecoders;

		public static IEnumerable<ISoundDecoder> Decoders { get { return soundDecoders; } }

		static SoundDecoders ()
		{
			soundDecoders = new List<ISoundDecoder> ();
		}

		public static void AddDecoder ( Type decoderType )
		{
			soundDecoders.Add ( Activator.CreateInstance ( decoderType ) as ISoundDecoder );
		}

		public static ISoundDecoder GetSoundDecoder ( string fileFormat )
		{
			foreach ( ISoundDecoder soundDecoder in soundDecoders )
			{
				var attrs = soundDecoder.GetType ().GetCustomAttributes ( typeof ( FileFormatAttribute ), true );
				if ( attrs.Length == 0 ) return null;
				foreach ( string fileExtension in ( attrs [ 0 ] as FileFormatAttribute ).FileExtension )
					if ( fileExtension == fileFormat.ToUpper ().Trim () )
						return soundDecoder;
			}

			return null;
		}

		public static SoundData? GetSoundData ( Stream stream )
		{
			foreach ( ISoundDecoder soundDecoder in SoundDecoders.Decoders )
			{
				try
				{
					stream.Position = 0;
					SoundData? soundData = soundDecoder.Decode ( stream );
					if ( soundData != null )
					{
						SoundData returnData = soundData.Value;
						returnData.SoundDecoder = soundDecoder;
						returnData.SoundStream = stream;
						return returnData;
					}
					else return null;
				}
				catch ( FileFormatMismatchException fileFormatMismatchException )
				{
					fileFormatMismatchException.ToString ();
					continue;
				}
				catch { return null; }
			}
			return null;
		}

		public static SoundData? GetSoundData<T> ( Stream stream ) where T : ISoundDecoder
		{
			try
			{
				ISoundDecoder decoder = Activator.CreateInstance<T> ();
				return decoder.Decode ( stream );
			}
			catch { return null; }
		}

		public static SoundData? GetSoundData ( ISoundDecoder soundDecoder, Stream stream )
		{
			try
			{
				return soundDecoder.Decode ( stream );
			}
			catch { return null; }
		}

		public static void AddDefaultDecoders ()
		{
			AddDecoder ( typeof ( DefaultWaveDecoder ) );
			AddDecoder ( typeof ( DefaultOggVobisDecoder ) );
			AddDecoder ( typeof ( DirectChunkDecoder ) );
		}
	}
}
