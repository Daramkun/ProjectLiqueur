using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;

namespace Daramkun.Liqueur.Audio
{
	class AudioDevice : IAudioDevice
	{
		public object Handle
		{
			get { throw new NotImplementedException (); }
		}

		public void Dispose ()
		{
			throw new NotImplementedException ();
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
			throw new NotImplementedException ();
		}

		public IAudio CreateAudio ( AudioInfo audioInfo )
		{
			throw new NotImplementedException ();
		}
	}
}
