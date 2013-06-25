using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Inputs.States
{
	public struct TouchState
	{
		TouchPoint [] touchPoints;

		TouchPoint [] TouchPoints { get { return touchPoints; } }

		internal TouchState ( params TouchPoint [] touchPoints )
		{
			this.touchPoints = touchPoints;
		}
	}
}
