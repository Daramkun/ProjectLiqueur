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

		public object Handle { get { return dSound; } }

		public AudioDevice ( IWindow window )
		{
			dSound = new SharpDX.DirectSound.DirectSound ();
			dSound.SetCooperativeLevel ( ( window.Handle as Form ).Handle, SharpDX.DirectSound.CooperativeLevel.Normal );
		}

		public void Dispose ()
		{
			dSound.Dispose ();
		}

		public void Play ( IAudio audio )
		{
			throw new NotImplementedException ();
		}

		public void Pause ( IAudio audio )
		{
			throw new NotImplementedException ();
		}

		public void Stop ( IAudio audio )
		{
			throw new NotImplementedException ();
		}

		public void Update ()
		{

		}

		public IAudio CreateAudio ( Contents.AudioInfo audioInfo )
		{
			throw new NotImplementedException ();
		}
	}
}
