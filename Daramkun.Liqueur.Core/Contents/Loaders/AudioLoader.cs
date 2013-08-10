using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Audio;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents.Decoder.Audios;
using Daramkun.Liqueur.Exceptions;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class AudioLoader : IContentLoader
	{
		public static List<IAudioDecoder> Decoders { get; private set; }

		static AudioLoader ()
		{
			Decoders = new List<IAudioDecoder> ();
		}

		public static void AddDefaultDecoders ()
		{
			Decoders.Add ( new WaveDecoder () );
			Decoders.Add ( new OggVorbisDecoder () );
		}

		public Type ContentType { get { return typeof ( IAudio ); } }

		public IEnumerable<string> FileExtensions
		{
			get
			{
				foreach ( IAudioDecoder decoder in Decoders )
					foreach ( object attr in decoder.GetType ().GetCustomAttributes ( typeof ( FileFormatAttribute ), true ) )
						foreach ( string ext in ( attr as FileFormatAttribute ).FileExtension )
							yield return ext;
			}
		}

		public bool IsSelfStreamDispose { get { return true; } }

		public object Load ( Stream stream, params object [] args )
		{
			bool isLoadComplete = false;
			AudioInfo audioInfo = new AudioInfo ();
			foreach ( IAudioDecoder decoder in Decoders )
			{
				try
				{
					audioInfo = decoder.Decode ( stream );
					isLoadComplete = true;
					break;
				}
				catch { }
			}
			if ( isLoadComplete )
				return LiqueurSystem.AudioDevice.CreateAudio ( audioInfo );
			else throw new FileFormatMismatchException ();
		}
	}
}
