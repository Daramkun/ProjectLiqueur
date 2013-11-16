using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daramkun.Liqueur.Contents;

namespace Daramkun.Liqueur.Audio
{
	class Audio : IAudio
	{
		SharpDX.DirectSound.SecondarySoundBuffer soundBuffer;
		WeakReference audioDevice;
		AudioInfo audioInfo;

		internal bool isPlaying = false;
		TimeSpan lastPosition;

		int offset;
		int totalLength;

		public object Handle { get { return soundBuffer; } }

		public TimeSpan Position
		{
			get
			{
				int playCursor, writeCursor;
				soundBuffer.GetCurrentPosition ( out playCursor, out writeCursor );
				return TimeSpan.FromSeconds ( playCursor / ( ( int ) audioInfo.AudioChannel * audioInfo.BitPerSample * audioInfo.SampleRate ) );
			}
			set
			{
				soundBuffer.CurrentPosition = ( int ) ( value.TotalSeconds *
					( ( int ) audioInfo.AudioChannel * audioInfo.BitPerSample * audioInfo.SampleRate ) );
			}
		}

		public TimeSpan Duration
		{
			get { return audioInfo.Duration; }
		}

		public bool IsPlaying
		{
			get { return soundBuffer.Status == 1; }
		}

		public bool IsPaused
		{
			get { return soundBuffer.Status == 0; }
		}

		public float Volume
		{
			get { return soundBuffer.Volume / 10000.0f; }
			set { soundBuffer.Volume = ( int ) ( value * 10000.0f ); }
		}

		public float Balance
		{
			get { return soundBuffer.Pan / 10000.0f; }
			set { soundBuffer.Pan = ( int ) ( value * 10000.0f ); }
		}

		public Audio ( IAudioDevice audioDevice, AudioInfo audioInfo )
		{
			this.audioDevice = new WeakReference ( audioDevice );
			this.audioInfo = audioInfo;

			SharpDX.DirectSound.SoundBufferDescription bufferDesc = new SharpDX.DirectSound.SoundBufferDescription ()
			{
				Flags = SharpDX.DirectSound.BufferFlags.ControlVolume | SharpDX.DirectSound.BufferFlags.ControlPan |
				SharpDX.DirectSound.BufferFlags.ControlPositionNotify | SharpDX.DirectSound.BufferFlags.StickyFocus |
				SharpDX.DirectSound.BufferFlags.Software | SharpDX.DirectSound.BufferFlags.GetCurrentPosition2 |
				SharpDX.DirectSound.BufferFlags.ControlFrequency,
				Format = new SharpDX.Multimedia.WaveFormat ( audioInfo.SampleRate, audioInfo.BitPerSample * 8, ( int ) audioInfo.AudioChannel ),
				BufferBytes = totalLength = ( int ) ( audioInfo.Duration.TotalSeconds *
					( ( int ) audioInfo.AudioChannel * audioInfo.BitPerSample * audioInfo.SampleRate ) ),
			};
			soundBuffer = new SharpDX.DirectSound.SecondarySoundBuffer ( audioDevice.Handle as SharpDX.DirectSound.DirectSound,
				bufferDesc );

			( audioDevice as AudioDevice ).audioList.Add ( this );

			offset = 0;
		}

		public void Dispose ()
		{
			soundBuffer.Dispose ();
		}

		public bool Update ()
		{
			if ( soundBuffer == null ) return true;
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
			SharpDX.DataStream secondPart;
			SharpDX.DataStream stream = soundBuffer.Lock ( offset, data.Length, SharpDX.DirectSound.LockFlags.EntireBuffer, out secondPart );
			stream.WriteRange<byte> ( data );
			soundBuffer.Unlock ( stream, secondPart );
			offset += data.Length;
		}

		public event EventHandler<System.ComponentModel.CancelEventArgs> BufferEnded;
	}
}
