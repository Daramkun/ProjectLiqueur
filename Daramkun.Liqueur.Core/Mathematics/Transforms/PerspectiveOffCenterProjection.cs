using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public struct PerspectiveOffCenterProjection : IProjection
	{
		public Rectangle OffCenter { get; set; }
		public float Near { get; set; }
		public float Far { get; set; }

		public PerspectiveOffCenterProjection ( int left, int top, int right, int bottom, float near, float far )
			: this ()
		{
			OffCenter = new Rectangle ( new Vector2 ( left, top ),
				new Vector2 ( right - left, bottom - top ) );
			Near = near;
			Far = far;
		}

		public PerspectiveOffCenterProjection ( int width, int height, float near, float far )
			: this ()
		{
			OffCenter = new Rectangle ( new Vector2 (), new Vector2 ( width, height ) );
			Near = near;
			Far = far;
		}

		public Matrix4x4 Matrix
		{
			get
			{
				Matrix4x4 matrix = new Matrix4x4 ();
				matrix.M11 = ( 2 * Near ) / ( OffCenter.Size.X );
				matrix.M22 = ( 2 * Near ) / ( -OffCenter.Size.Y );
				matrix.M31 = ( OffCenter.Position.X + ( OffCenter.Position.X + OffCenter.Size.X ) ) /
					( -OffCenter.Size.X );
				matrix.M32 = ( OffCenter.Position.Y + ( OffCenter.Position.Y + OffCenter.Size.Y ) ) /
					( -OffCenter.Size.Y );
				matrix.M33 = Far / ( Near - Far );
				matrix.M34 = -1;
				matrix.M43 = ( Near * Far ) / ( Near - Far );
				return matrix;
			}
		}
	}
}
