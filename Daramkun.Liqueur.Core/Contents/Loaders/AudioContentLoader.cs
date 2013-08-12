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
	/// <summary>
	/// Audio Content Loader class
	/// </summary>
	public class AudioContentLoader : IContentLoader
	{
		/// <summary>
		/// Audio Decoders
		/// </summary>
		public static List<IAudioDecoder> Decoders { get; private set; }

		static AudioContentLoader ()
		{
			Decoders = new List<IAudioDecoder> ();
		}

		/// <summary>
		/// Add Default Decoders
		/// (WAV, OGG)
		/// </summary>
		public static void AddDefaultDecoders ()
		{
			Decoders.Add ( new WaveDecoder () );
			Decoders.Add ( new OggVorbisDecoder () );
		}

		/// <summary>
		/// Content Type (IAudio)
		/// </summary>
		public Type ContentType { get { return typeof ( IAudio ); } }

		/// <summary>
		/// File Extensions
		/// </summary>
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

		/// <summary>
		/// Is loaded content will be auto disposing?
		/// </summary>
		public bool IsSelfStreamDispose { get { return true; } }

		/// <summary>
		/// Load Audio Content
		/// </summary>
		/// <param name="stream">Audio stream</param>
		/// <param name="args">This parameter must be empty</param>
		/// <returns>Loaded audio</returns>
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
