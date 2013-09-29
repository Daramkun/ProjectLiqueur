using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using OpenTK.Audio.OpenAL;

namespace Daramkun.Liqueur.Audio
{
	class Audio : IAudio
	{
		internal int sourceId;
		List<int> bufferIds;

		AudioInfo audioInfo;
		ALFormat alFormat;

		internal bool isPlaying = false;
		TimeSpan lastPosition;

		WeakReference audioDevice;

		public object Handle { get { return sourceId; } }

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

		public TimeSpan Duration { get { return audioInfo.Duration; } }

		public bool IsPlaying { get { return AL.GetSourceState ( sourceId ) == ALSourceState.Playing; } }
		public bool IsPaused { get { return AL.GetSourceState ( sourceId ) == ALSourceState.Paused; } }

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

		public float Balance
		{
			get
			{
				float volume;
				AL.GetSource ( sourceId, ALSourcef.Pitch, out volume );
				return volume;
			}
			set
			{
				if ( value < 0f || value > 1f )
					throw new ArgumentException ();
				AL.Source ( sourceId, ALSourcef.Pitch, value );
			}
		}

		public Audio ( IAudioDevice audioDevice, AudioInfo audioInfo )
		{
			this.audioDevice = new WeakReference ( audioDevice );
			this.audioInfo = audioInfo;

			bufferIds = new List<int> ();
			sourceId = AL.GenSource ();
			alFormat = ( ( audioInfo.AudioChannel == AudioChannel.Stereo ) ?
				( ( audioInfo.BitPerSample == 2 ) ? ALFormat.Stereo16 : ALFormat.Stereo8 ) :
				( ( audioInfo.BitPerSample == 2 ) ? ALFormat.Mono16 : ALFormat.Mono8 ) );

			( audioDevice as AudioDevice ).audioList.Add ( this );
		}

		~Audio ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				AL.DeleteSource ( sourceId );
				sourceId = 0;
				foreach ( int bufferId in bufferIds )
					if ( bufferId != 0 )
						AL.DeleteBuffer ( bufferId );
				bufferIds.Clear ();
			}
			if ( audioDevice.Target != null )
			{
				if ( ( audioDevice.Target as AudioDevice ).audioList.Contains ( this ) )
					( audioDevice.Target as AudioDevice ).audioList.Remove ( this );
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		public bool Update ()
		{
			if ( sourceId == 0 ) return true;
			byte [] data = audioInfo.GetSample ( null );
			if ( data == null )
			{
				if ( isPlaying && !IsPlaying )
				{
					if ( BufferEnded != null )
					{
						CancelEventArgs cancelEvent = new CancelEventArgs ();
						BufferEnded ( this, cancelEvent );
						if ( cancelEvent.Cancel ) return true;
						else return false;
					}
					else return true;
				}
				else return true;
			}

			BufferData ( data );

			if ( isPlaying && !IsPlaying )
			{
				( audioDevice.Target as AudioDevice ).Play ( this );
				Position = lastPosition;
			}
			lastPosition = Position;
			return false;
		}

		public void BufferData ( byte [] data )
		{
			int bufferId = AL.GenBuffer ();
			AL.BufferData ( bufferId, alFormat, data, data.Length, audioInfo.SampleRate );
			AL.SourceQueueBuffer ( sourceId, bufferId );
			bufferIds.Add ( bufferId );
		}

		public event EventHandler<CancelEventArgs> BufferEnded;
	}
}
