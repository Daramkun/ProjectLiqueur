using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Walnut.Nodes
{
	[Flags]
	public enum ObjectOffset : byte
	{
		Top = 0 << 0,
		Center = 1 << 0,
		Bottom = 1 << 2,

		Left = 0 << 0,
		Middle = 1 << 1,
		Right = 1 << 3,

		TopLeft = Top | Left,
		TopMiddle = Top | Middle,
		TopRight = Top | Right,
		
		CenterLeft = Center | Left,
		CenterMiddle = Center | Middle,
		CenterRight = Center | Right,

		BottomLeft = Bottom | Left,
		BottomMiddle = Bottom | Middle,
		BottomRight = Bottom | Right,
	}
}
