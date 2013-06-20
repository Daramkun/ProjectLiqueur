using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;

namespace Daramkun.Liqueur.Graphics
{
	public interface IRenderer
	{
		Vector2 [] AvailableScreenSize { get; }
		Vector2 ScreenSize { get; set; }
		bool FullscreenMode { get; set; }

		void Clear ( Color color );
		void Present ();
	}
}