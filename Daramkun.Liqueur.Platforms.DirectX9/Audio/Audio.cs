using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramkun.Liqueur.Audio
{
	class Audio : IAudio
	{
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

		public TimeSpan Duration
		{
			get { throw new NotImplementedException (); }
		}

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
			get
			{
				throw new NotImplementedException ();
			}
			set
			{
				throw new NotImplementedException ();
			}
		}

		public float Balance
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

		public Audio ( IAudioDevice audioDevice )
		{

		}

		public object Handle
		{
			get { throw new NotImplementedException (); }
		}

		public bool Update ()
		{
			throw new NotImplementedException ();
		}

		public void BufferData ( byte [] data )
		{
			throw new NotImplementedException ();
		}

		public event EventHandler<System.ComponentModel.CancelEventArgs> BufferEnded;

		public void Dispose ()
		{
			throw new NotImplementedException ();
		}
	}
}
