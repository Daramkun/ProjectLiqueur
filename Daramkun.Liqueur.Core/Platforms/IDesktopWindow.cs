using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Platforms
{
	public interface IDesktopWindow
	{
		bool IsCursorVisible { get; set; }
		bool IsResizable { get; set; }
		object Icon { get; set; }
	}
}
