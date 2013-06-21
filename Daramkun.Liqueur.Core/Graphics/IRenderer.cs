using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;

namespace Daramkun.Liqueur.Graphics
{
	public enum CullingMode
	{
		None,
		ClockWise,
		CounterClockWise,
	}

	public interface IRenderer : IDisposable
	{
		Vector2 [] AvailableScreenSize { get; }
		Vector2 ScreenSize { get; set; }
		bool FullscreenMode { get; set; }

		CullingMode CullingMode { get; set; }

		void Clear ( Color color );
		void Present ();

		void DrawPrimitive<T> ( Primitive<T> primitive );

		IImage CreateImage ( ImageData imageData, Color colorKey );
	}
}