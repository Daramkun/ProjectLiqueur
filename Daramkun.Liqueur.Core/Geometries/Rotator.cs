using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Geometries
{
	public static class Rotator
	{
		public static Vector2 Rotation ( Vector2 basePosition, float radian )
		{
			return new Vector2 (
				basePosition.X * ( float ) Math.Cos ( radian ) - basePosition.Y * ( float ) Math.Sin ( radian ),
				basePosition.X * ( float ) Math.Sin ( radian ) + basePosition.Y * ( float ) Math.Cos ( radian )
			);
		}
	}
}
