using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Audio
{
	class AudioDevice : IAudioDevice
	{
		SharpDX.DirectSound.DirectSound dSound;
		SharpDX.DirectSound.PrimarySoundBuffer soundBuffer;
		internal List<IAudio> audioList;

		public object Handle { get { return dSound; } }

		public AudioDevice ( IWindow window )
		{
			dSound = new SharpDX.DirectSound.DirectSound ();
			dSound.SetCooperativeLevel ( ( window.Handle as Form ).Handle, SharpDX.DirectSound.CooperativeLevel.Priority );

			soundBuffer = new SharpDX.DirectSound.PrimarySoundBuffer ( dSound, new SharpDX.DirectSound.SoundBufferDescription ()
			{
				Flags = SharpDX.DirectSound.BufferFlags.PrimaryBuffer,
			} );

			soundBuffer.Play ( 0, SharpDX.DirectSound.PlayFlags.Looping );

			audioList = new List<IAudio> ();
		}

		public void Dispose ()
		{
			soundBuffer.Dispose ();
			dSound.Dispose ();
		}

		public void Play ( IAudio audio )
		{
			( audio.Handle as SharpDX.DirectSound.SecondarySoundBuffer ).Play ( 0, SharpDX.DirectSound.PlayFlags.None );
		}

		public void Pause ( IAudio audio )
		{
			( audio.Handle as SharpDX.DirectSound.SecondarySoundBuffer ).Stop ();
		}

		public void Stop ( IAudio audio )
		{
			( audio.Handle as SharpDX.DirectSound.SecondarySoundBuffer ).Stop ();
			audio.Position = new TimeSpan ();
		}

		public void Update ()
		{
			if ( audioList.Count > 0 )
				foreach ( IAudio audio in audioList.ToArray () )
					audio.Update ();
		}

		public IAudio CreateAudio ( Contents.AudioInfo audioInfo )
		{
			return new Audio ( this, audioInfo );
		}
	}
}
