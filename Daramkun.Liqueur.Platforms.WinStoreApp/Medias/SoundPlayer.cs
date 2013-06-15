using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Decoders;
using Daramkun.Liqueur.Decoders.Sounds;
using Daramkun.Liqueur.Exceptions;
#if OPENTK
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;
#elif XNA
using Microsoft.Xna.Framework.Audio;
#endif

namespace Daramkun.Liqueur.Medias
{
	/*
	public class SoundPlayer : ISoundPlayer
	{
#if OPENTK
		static AudioContext audioContext;
		static uint audioContextRef;

		int sourceId;
		List<int> bufferIds;
		ALFormat alFormat;
#elif XNA
		DynamicSoundEffectInstance soundEffect;
#endif

		SoundData soundData;

		bool isPlaying = false;
		TimeSpan lastPosition;

		public TimeSpan Position
		{
			get
			{
#if OPENTK
				float seconds;
				AL.GetSource ( sourceId, ALSourcef.SecOffset, out seconds );
				return TimeSpan.FromSeconds ( seconds );
#endif
			}
			set
			{
#if OPENTK
				if ( value > Duration ) throw new ArgumentException ();
				AL.Source ( sourceId, ALSourcef.SecOffset, ( float ) value.TotalSeconds );
#endif
			}
		}

		public TimeSpan Duration { get { return soundData.Duration; } }

		public float Volume
		{
			get
			{
#if OPENTK
				float volume;
				AL.GetSource ( sourceId, ALSourcef.Gain, out volume );
				return volume;
#endif
			}
			set
			{
#if OPENTK
				if ( value < 0f || value > 1f )
					throw new ArgumentException ();
				AL.Source ( sourceId, ALSourcef.Gain, value );
#endif
			}
		}

		public bool IsPlaying
		{
			get
			{
#if OPENTK
				return AL.GetSourceState ( sourceId ) == ALSourceState.Playing;
#endif
			}
		}
		public bool IsPaused
		{
			get
			{
#if OPENTK
				return AL.GetSourceState ( sourceId ) == ALSourceState.Paused;
#endif
			}
		}

#if OPENTK
		private void InitializeAudioContext ()
		{
			if ( audioContext == null )
			{
				audioContext = new AudioContext ( AudioContext.DefaultDevice );
				if ( audioContext == null )
					throw new AudioFailedInitializeException ();
			}
			++audioContextRef;
		}

		private void InitializeALSource ()
		{
			bufferIds = new List<int> ();
			sourceId = AL.GenSource ();
			if ( sourceId == 0 ) throw new AudioFailedInitializeException ();

			alFormat = ( ( soundData.AudioChannel == AudioChannel.Stereo ) ?
				( ( soundData.BitPerSample == BitPerSample.Bps16 ) ? ALFormat.Stereo16 : ALFormat.Stereo8 ) :
				( ( soundData.BitPerSample == BitPerSample.Bps16 ) ? ALFormat.Mono16 : ALFormat.Mono8 ) );
		}
#endif

		public SoundPlayer ( Stream stream )
		{
			InitializeSoundPlayer ( stream, false );
		}

		public SoundPlayer ( Stream stream, bool fullLoading )
		{
			InitializeSoundPlayer ( stream, fullLoading );
		}

		public SoundPlayer ( SoundData soundData, bool fullLoading )
		{
			this.soundData = soundData;

#if OPENTK
			InitializeAudioContext ();
			InitializeALSource ();
#elif XNA
			soundEffect = new DynamicSoundEffectInstance ( soundData.SampleRate,
				soundData.AudioChannel == AudioChannel.Mono ? Microsoft.Xna.Framework.Audio.AudioChannels.Mono :
				Microsoft.Xna.Framework.Audio.AudioChannels.Stereo );
#endif

			Update ();
			if ( fullLoading )
				while ( !Update () ) ;
		}

		~SoundPlayer ()
		{
			Dispose ( false );
		}

		private void InitializeSoundPlayer ( Stream stream, bool fullLoading )
		{
			SoundData? soundChunk = SoundDecoders.GetSoundData ( stream );

			if ( soundChunk == null )
				throw new FileFormatMismatchException ();
			soundData = soundChunk.Value;

#if OPENTK
			InitializeAudioContext ();
			InitializeALSource ();
#endif

			Update ();
			if ( fullLoading )
				while ( !Update () ) ;
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
#if OPENTK
				if ( sourceId != 0 )
					AL.DeleteSource ( sourceId );
				sourceId = 0;
				foreach ( int bufferId in bufferIds )
					if ( bufferId != 0 )
						AL.DeleteBuffer ( bufferId );
				bufferIds.Clear ();

				if ( audioContextRef == 0 ) return;
				--audioContextRef;
				if ( audioContextRef == 0 )
				{
					audioContext.Dispose ();
					audioContext = null;
				}
#elif XNA
				soundEffect.Dispose ();
				soundEffect = null;
#endif
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( true );
		}

		public void Play ()
		{
			isPlaying = true;
#if OPENTK
			AL.SourcePlay ( sourceId );
#elif XNA
			soundEffect.Play ();
#endif
		}

		public void Stop ()
		{
			isPlaying = false;
#if OPENTK
			AL.SourceStop ( sourceId );
#elif XNA
			soundEffect.Stop ();
#endif
		}

		public void Pause ()
		{
			isPlaying = false;
#if OPENTK
			AL.SourcePause ( sourceId );
#elif XNA
			soundEffect.Pause ();
#endif
		}

		public SoundData SoundData { get { return soundData; } }

		public bool Update ()
		{
			byte [] data = soundData.SoundDecoder.GetSample ( soundData );
			if ( data == null ) return true;

#if OPENTK
			if ( audioContext == null ) throw new Exception ();
			int bufferId = AL.GenBuffer ();
			AL.BufferData ( bufferId, alFormat, data, data.Length, soundData.SampleRate );
			AL.SourceQueueBuffer ( sourceId, bufferId );
			bufferIds.Add ( bufferId );
#endif

			if ( isPlaying && !IsPlaying )
			{
				Play ();
				Position = lastPosition;
			}
			lastPosition = Position;
			return false;
		}
	}
	*/
}
