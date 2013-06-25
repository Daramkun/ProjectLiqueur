using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Graphics.Vertices;
using Daramkun.Liqueur.Math;

namespace Daramkun.Liqueur.Graphics
{
	public enum CullingMode
	{
		None,
		ClockWise,
		CounterClockWise,
	}

	public enum FillMode
	{
		Point,
		Wireframe,
		Solid,
	}

	public interface IRenderer : IDisposable
	{
		Vector2 [] AvailableScreenSize { get; }
		Vector2 ScreenSize { get; set; }
		bool FullscreenMode { get; set; }

		CullingMode CullingMode { get; set; }
		FillMode FillMode { get; set; }
		Viewport Viewport { get; set; }

		void Clear ( Color color );
		void Present ();

		void DrawPrimitive<T> ( IPrimitive<T> primitive ) where T : IFlexibleVertex;

		IImage CreateImage ( ImageData imageData, Color colorKey );
		IPrimitive<T> CreatePrimitive<T> ( int vertexCount, int indexCount ) where T : IFlexibleVertex;
		IPrimitive<T> CreatePrimitive<T> ( T [] vertexArray, int [] indexArray ) where T : IFlexibleVertex;
	}
}