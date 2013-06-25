using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Inputs.RawDevices;
using Daramkun.Liqueur.Platforms;

namespace Daramkun.Liqueur.Inputs.Devices
{
	public class TouchPanel : RawTouchPanel
	{
		public TouchPanel ( IWindow window )
			: base ( window )
		{
			
		}
	}
}
