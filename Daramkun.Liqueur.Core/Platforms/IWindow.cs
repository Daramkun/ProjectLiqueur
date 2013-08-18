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
		Vector2 ClientSize { get; }
		object Handle { get; }

		void DoEvents ();
	}
}
