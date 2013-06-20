using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Math
{
	public struct Rectangle : ICollision<Rectangle>, ICollision<Circle>, ICollision<Vector2>
	{
		public Vector2 Position;
		public Vector2 Size;

		//public Vector2 Position { get { return position; } set { position = value; } }
		//public Vector2 Size { get { return size; } set { size = value; } }

		public Rectangle ( float x, float y, float width, float height )
		{
			Position = new Vector2 ( x, y );
			Size = new Vector2 ( width, height );
		}

		public Rectangle ( Vector2 position, Vector2 size )
		{
			this.Position = position;
			this.Size = size;
		}

		public override string ToString ()
		{
			return String.Format ( "{{X:{0}, Y:{1}, Width:{2}, Height:{3}}}",
				Position.X, Position.Y, Size.X, Size.Y );
		}

		public bool IsCollisionTo ( Rectangle obj )
		{
			return ( Position.X - Size.X / 2 <= obj.Position.X + obj.Size.X / 2 &&
				obj.Position.X - Size.X / 2 <= Position.X + Size.X / 2 &&
				Position.Y - Size.Y / 2 <= obj.Position.Y + obj.Size.Y / 2 &&
				obj.Position.Y - Size.Y / 2 <= Position.Y + Size.Y / 2 );
		}

		public bool IsCollisionTo ( Circle obj )
		{
			return obj.IsCollisionTo ( this );
		}

		public bool IsCollisionTo ( Vector2 obj )
		{
			return IsCollisionTo ( new Rectangle ( obj, new Vector2 () ) );
		}
	}
}
