using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	public struct Viewport
	{
		public int X { get; set; }
		public int Y { get; set; }
		public int Width { get; set; }
		public int Height { get; set; }

		public override string ToString ()
		{
			return string.Format ( "{{X:{0}, Y:{1}, Width:{2}, Height:{3}}}",
				X, Y, Width, Height );
		}
	}
}