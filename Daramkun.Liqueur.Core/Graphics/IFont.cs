using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Geometries;

namespace Daramkun.Liqueur.Graphics
{
	public interface IFont : IDisposable
	{
		string FontFamily { get; }
		float FontSize { get; }

		void DrawFont ( string text, Color color, Vector2 position );
		void DrawFont ( string text, Color color, Vector2 position, Vector2 area );

		Vector2 MeasureString ( string text );
		int MeasureString ( string text, Vector2 area );
	}
}
