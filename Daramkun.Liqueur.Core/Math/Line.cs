using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Math
{
	public struct Line : ICollision<Line>
	{
		public Vector2 Point1, Point2;

		//public Vector2 Point1 { get { return point1; } set { point1 = value; } }
		//public Vector2 Point2 { get { return point2; } set { point2 = value; } }

		public Line ( float x1, float y1, float x2, float y2 )
		{
			Point1 = new Vector2 ( x1, y1 );
			Point2 = new Vector2 ( x2, y2 );
		}

		public Line ( Vector2 p1, Vector2 p2 )
		{
			Point1 = p1;
			Point2 = p2;
		}

		public bool IsCollisionTo ( Line obj )
		{
			float d = ( Point1.X - Point2.X ) * ( obj.Point1.X - obj.Point2.X ) -
				( Point1.Y - Point2.Y ) * ( obj.Point1.Y - obj.Point2.Y );
			if ( d == 0 ) return false;

			float pre = ( Point1.X * Point2.Y - Point1.Y * Point2.X ),
				post = ( obj.Point1.X * obj.Point2.Y - obj.Point1.Y * obj.Point2.X );
			float x = ( pre * ( obj.Point1.X - obj.Point2.X ) - ( Point1.X - Point2.X ) * post ) / d;
			float y = ( pre * ( obj.Point1.Y - obj.Point2.Y ) - ( Point1.Y - Point2.Y ) * post ) / d;

			if ( x < System.Math.Min ( Point1.X, Point2.X ) || x > System.Math.Max ( Point1.X, Point2.X ) ||
				x < System.Math.Min ( obj.Point1.X, obj.Point2.X ) || x > System.Math.Max ( obj.Point1.X, obj.Point2.X ) )
				return false;
			if ( y < System.Math.Min ( Point1.Y, Point2.Y ) || y > System.Math.Max ( Point1.Y, Point2.Y ) ||
				y < System.Math.Min ( obj.Point1.Y, obj.Point2.Y ) || y > System.Math.Max ( obj.Point1.Y, obj.Point2.Y ) )
				return false;

			return true;
		}
	}
}
