using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Platforms
{
	public interface IWindow : IDisposable
	{
		string Title { get; set; }
		bool IsCursorVisible { get; set; }
		bool IsResizable { get; set; }
		object Icon { get; set; }
		Vector2 ClientSize { get; }
		object Handle { get; }

		void DoEvents ();
	}
}
