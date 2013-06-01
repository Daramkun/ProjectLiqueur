using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.States;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Inputs.RawDevices
{
	public abstract class RawKeyboard : IInputDevice<KeyboardState>
	{
		public virtual bool IsConnected { get { return false; } }

		protected List<KeyboardKey> PressedKeys { get; private set; }

		public RawKeyboard ( IWindow window )
		{
			PressedKeys = new List<KeyboardKey> ();
		}

		~RawKeyboard ()
		{
			Dispose ( false );
		}

		public virtual KeyboardState GetState ()
		{
			return new KeyboardState ( PressedKeys.ToArray () );
		}

		protected virtual void Dispose ( bool isDisposing )
		{

		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( true );
		}
	}
}
