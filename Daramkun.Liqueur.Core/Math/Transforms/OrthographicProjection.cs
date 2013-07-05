using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Math.Transforms
{
	public struct OrthographicProjection : IProjection
	{
		public float Near { get; set; }
		public float Far { get; set; }
		public Rectangle Area { get; set; }

		public OrthographicProjection ( int width, int height, float near, float far )
			: this ()
		{
			Area = new Rectangle ( new Vector2 (), new Vector2 ( width, height ) );
			Near = near;
			Far = far;
		}

		public OrthographicProjection ( int left, int top, int right, int bottom, float near, float far )
			: this ()
		{
			Area = new Rectangle ( new Vector2 ( left, top ), new Vector2 ( right - left, bottom - top ) );
			Near = near;
			Far = far;
		}

		public Matrix4x4 Matrix
		{
			get
			{
				Matrix4x4 matrix = new Matrix4x4 ();
				matrix.M11 = 2 / Area.Size.X;
				matrix.M22 = 2 / Area.Size.Y;
				matrix.M33 = 1 / ( Near - Far );
				matrix.M41 = ( Area.Position.X + (Area.Position.X + Area.Size.X) ) /
					( Area.Position.X - ( Area.Position.X + Area.Size.X ) );
				matrix.M42 = ( Area.Position.Y + ( Area.Position.Y + Area.Size.Y ) ) /
					( ( Area.Position.Y + Area.Size.Y ) - Area.Position.Y );
				matrix.M43 = Near / ( Near - Far );
				matrix.M44 = 1;
				return matrix;
			}
		}
	}
}
