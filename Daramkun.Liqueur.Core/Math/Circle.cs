using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Math
{
	public struct Circle : ICollision<Circle>, ICollision<Rectangle>, ICollision<Vector2>
	{
		public Vector2 Position;
		public float Radius;

		//public Vector2 Position { get { return position; } }
		//public float Radius { get { return radius; } }

		public Circle ( float x, float y, float radius )
		{
			Position = new Vector2 ( x, y );
			this.Radius = radius;
		}

		public Circle ( Vector2 position, float radius )
		{
			this.Position = position;
			this.Radius = radius;
		}

		public override string ToString ()
		{
			return String.Format ( "{{X:{0}, Y:{1}, Radius:{2}}}",
				Position.X, Position.Y, Radius );
		}

		public bool IsCollisionTo ( Circle obj )
		{
			return ( Vector2.Distance ( Position, obj.Position ) <= Radius + obj.Radius );
		}

		public bool IsCollisionTo ( Rectangle obj )
		{
			float [] rcp = new float [ 4 ];
			float [] temp = new float [ 2 ];
			float min;

			float x = Position.X, y = Position.Y, r = Radius;
			float left = obj.Position.X - obj.Size.X / 2, right = obj.Position.X + obj.Size.X / 2,
				top = obj.Position.Y - obj.Size.Y / 2, bottom = obj.Position.X + obj.Size.Y / 2;

			rcp [ 0 ] = ( float ) System.Math.Sqrt ( System.Math.Pow ( x - left, 2 ) + System.Math.Pow ( y - top, 2 ) );
			rcp [ 1 ] = ( float ) System.Math.Sqrt ( System.Math.Pow ( x - left, 2 ) + System.Math.Pow ( y - bottom, 2 ) );
			rcp [ 2 ] = ( float ) System.Math.Sqrt ( System.Math.Pow ( x - right, 2 ) + System.Math.Pow ( y - top, 2 ) );
			rcp [ 3 ] = ( float ) System.Math.Sqrt ( System.Math.Pow ( x - right, 2 ) + System.Math.Pow ( y - bottom, 2 ) );

			if ( rcp [ 0 ] < rcp [ 1 ] ) temp [ 0 ] = rcp [ 0 ];
			else temp [ 0 ] = rcp [ 1 ];
			if ( rcp [ 2 ] < rcp [ 3 ] ) temp [ 1 ] = rcp [ 2 ];
			else temp [ 1 ] = rcp [ 3 ];

			if ( temp [ 0 ] < temp [ 1 ] ) min = temp [ 0 ];
			else min = temp [ 1 ];

			if ( left <= x + r && top <= y - r && right >= x - r && bottom >= y + r ) return true;
			else if ( left <= x - r && top <= y + r && right >= x + r && bottom >= y - r ) return true;
			else if ( min <= r ) return true;
			else return false;
		}

		public bool IsCollisionTo ( Vector2 obj )
		{
			return IsCollisionTo ( new Circle ( obj, 0 ) );
		}
	}
}
