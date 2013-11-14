using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public class OrthographicProjection : IProjection
	{
		public float Near { get; set; }
		public float Far { get; set; }
		public Rectangle Area { get; set; }

		public OrthographicProjection ( float width, float height, float near, float far )
			//: this ()
		{
			Area = new Rectangle ( new Vector2 (), new Vector2 ( width, height ) );
			Near = near;
			Far = far;
		}

		public OrthographicProjection ( float left, float right, float bottom, float top, float near, float far )
			//: this ()
		{
			Area = new Rectangle ( new Vector2 ( left, top ), new Vector2 ( right - left, bottom - top ) );
			Near = near;
			Far = far;
		}

		public Matrix4x4 Matrix
		{
			get
			{
				float left = Area.Position.X, right = Area.Position.X + Area.Size.X,
					top = Area.Position.Y, bottom = Area.Position.Y + Area.Size.Y;

				Matrix4x4 matrix = new Matrix4x4 ();
				matrix.M11 = 2 / ( right - left );
				matrix.M22 = 2 / ( top - bottom );
				matrix.M33 = 1 / ( Far - Near );
				matrix.M41 = ( left + right ) / ( left - right );
				matrix.M42 = ( top + bottom ) / ( bottom - top );
				matrix.M43 = Near / ( Near - Far );
				matrix.M44 = 1;
				return matrix;
			}
		}
	}
}
