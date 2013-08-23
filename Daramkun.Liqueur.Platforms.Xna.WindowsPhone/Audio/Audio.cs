using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Microsoft.Xna.Framework.Audio;

namespace Daramkun.Liqueur.Audio
{
	class Audio : IAudio
	{
		DynamicSoundEffectInstance soundEffect;
		AudioInfo audioInfo;

		public object Handle { get { return soundEffect; } }

		public TimeSpan Position
		{
			get
			{
				throw new NotImplementedException ();
			}
			set
			{
				throw new NotImplementedException ();
			}
		}

		public TimeSpan Duration { get { return audioInfo.Duration; } }

		public bool IsPlaying
		{
			get { throw new NotImplementedException (); }
		}

		public bool IsPaused
		{
			get { throw new NotImplementedException (); }
		}

		public float Volume
		{
			get { return soundEffect.Volume; }
			set { soundEffect.Volume = value; }
		}

		public float Balance
		{
			get { return soundEffect.Pitch; }
			set { soundEffect.Pitch = value; }
		}

		public void Dispose ()
		{
			throw new NotImplementedException ();
		}

		public bool Update ()
		{
			throw new NotImplementedException ();
		}

		public void BufferData ( byte [] data )
		{
			soundEffect.SubmitBuffer ( data );
		}

		public event EventHandler<CancelEventArgs> BufferEnded;
	}
}
