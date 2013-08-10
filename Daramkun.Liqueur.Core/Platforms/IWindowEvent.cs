using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Platforms
{
	public interface IWindowEvent
	{
		void WindowResize ();
		void WindowActivated ();
		void WindowDeactivated ();
	}
}
