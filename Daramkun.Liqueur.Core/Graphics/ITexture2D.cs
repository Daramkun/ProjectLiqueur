using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Math;

namespace Daramkun.Liqueur.Graphics
{
	public interface ITexture2D : IDisposable
	{
		int Width { get; }
		int Height { get; }
		Vector2 Size { get; }

		void DrawBitmap ( Color overlay, Transform2 transform );
		void DrawBitmap ( Color overlay, Transform2 transform, Rectangle sourceRectangle );
	}
}
