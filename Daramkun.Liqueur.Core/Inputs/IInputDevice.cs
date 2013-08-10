using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Inputs
{
	public interface IInputDevice<T> : IDisposable
	{
		bool IsSupport { get; }
		bool IsConnected { get; }

		T GetState ( PlayerIndex playerIndex = PlayerIndex.Player1 );
	}
}
