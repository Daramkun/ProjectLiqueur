using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Inputs.RawDevice
{
	public abstract class CommonDevice<T> : IInputDevice<T>
	{
		public abstract bool IsSupport { get; }
		public abstract bool IsConnected { get; }

		~CommonDevice ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{

		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		public abstract T GetState ( PlayerIndex playerIndex = PlayerIndex.Player1 );
	}
}
