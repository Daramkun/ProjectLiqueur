using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Inputs
{
	[Flags]
	public enum MouseButton : uint
	{
		Unknown = 0,

		Left = 1 << 0,
		Middle = 1 << 1,
		Right = 1 << 2,
	}
}
