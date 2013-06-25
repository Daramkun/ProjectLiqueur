using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Inputs
{
	[Flags]
	public enum GamePadButton : uint
	{
		None = 0,

		A = 1 << 0,
		B = 1 << 1,
		X = 1 << 2,
		Y = 1 << 3,

		Start = 1 << 4,
		Back = 1 << 5,

		DPadUp = 1 << 6,
		DPadDown = 1 << 7,
		DPadLeft = 1 << 8,
		DPadRight = 1 << 9,

		LeftBumper = 1 << 10,
		RightBumper = 1 << 11,
		LeftTrigger = 1 << 12,
		RightTrigger = 1 << 13,

		LeftThumbStick = 1 << 14,
		RightThumbStick = 1 << 15,
	}
}
