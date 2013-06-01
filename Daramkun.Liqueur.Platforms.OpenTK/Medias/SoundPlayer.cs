using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Decoders;
using Daramkun.Liqueur.Decoders.Sounds;
using Daramkun.Liqueur.Exceptions;
using OpenTK.Audio;
using OpenTK.Audio.OpenAL;

namespace Daramkun.Liqueur.Medias
{
	public class SoundPlayer : ISoundPlayer
	{
		static AudioContext audioContext;
		static uint audioContextRef;

		int sourceId;
		List<int> bufferIds;
		ALFormat alFormat;

		SoundData soundData;

		bool isPlaying = false;
		TimeSpan lastPosition;

		public TimeSpan Position
		{
			get
			{
				float seconds;
				AL.GetSource ( sourceId, ALSourcef.SecOffset, out seconds );
				return TimeSpan.FromSeconds ( seconds );
			}
			set
			{
				if ( value > Duration ) throw new ArgumentException ();
				AL.Source ( sourceId, ALSourcef.SecOffset, ( float ) value.TotalSeconds );
			}
		}

		public TimeSpan Duration { get { return soundData.Duration; } }

		public float Volume
		{
			get
			{
				float volume;
				AL.GetSource ( sourceId, ALSourcef.Gain, out volume );
				return volume;
			}
			set
			{
				if ( value < 0f || value > 1f )
					throw new ArgumentException ();
				AL.Source ( sourceId, ALSourcef.Gain, value );
			}
		}

		public bool IsPlaying
		{
			get
			{
				return AL.GetSourceState ( sourceId ) == ALSourceState.Playing;
			}
		}
		public bool IsPaused
		{
			get
			{
				return AL.GetSourceState ( sourceId ) == ALSourceState.Paused;
			}
		}

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

			InitializeAudioContext ();
			InitializeALSource ();

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

			InitializeAudioContext ();
			InitializeALSource ();

			Update ();
			if ( fullLoading )
				while ( !Update () ) ;
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
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
			AL.SourcePlay ( sourceId );
		}

		public void Stop ()
		{
			isPlaying = false;
			AL.SourceStop ( sourceId );
		}

		public void Pause ()
		{
			isPlaying = false;
			AL.SourcePause ( sourceId );
		}

		public SoundData SoundData { get { return soundData; } }

		public bool Update ()
		{
			byte [] data = soundData.SoundDecoder.GetSample ( soundData );
			if ( data == null ) return true;

			if ( audioContext == null ) throw new Exception ();
			int bufferId = AL.GenBuffer ();
			AL.BufferData ( bufferId, alFormat, data, data.Length, soundData.SampleRate );
			AL.SourceQueueBuffer ( sourceId, bufferId );
			bufferIds.Add ( bufferId );

			if ( isPlaying && !IsPlaying )
			{
				Play ();
				Position = lastPosition;
			}
			lastPosition = Position;
			return false;
		}
	}
}
