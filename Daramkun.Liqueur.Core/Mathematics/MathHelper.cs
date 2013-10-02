using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics
{
	public static class MathHelper
	{
		public const float PI = ( float ) Math.PI;
		public const float PIOver2 = PI / 2.0f;
		public const float PIOver4 = PI / 4.0f;
		public const float TwoPI = PI * 2.0f;

		public static float SquareRoot ( float x )
		{
			return ( float ) Math.Sqrt ( x );
		}

		public static float Power ( float x, float y )
		{
			return ( float ) Math.Pow ( x, y );
		}

		public static float ToDegrees ( float radians )
		{
			return ( radians * 180 ) / PI;
		}

		public static float ToRadians ( float degrees )
		{
			return ( degrees / 180 ) * PI;
		}

		public static float WrapAngle ( float angle )
		{
			angle = ( float ) Math.IEEERemainder ( ( double ) angle, 6.2831854820251465 );
			if ( angle <= -3.14159274f )
				angle += 6.28318548f;
			else if ( angle > 3.14159274f )
				angle -= 6.28318548f;
			return angle;
		}
	}
}
