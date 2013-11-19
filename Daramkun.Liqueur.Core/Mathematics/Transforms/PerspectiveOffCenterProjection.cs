using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public class PerspectiveOffCenterProjection : IProjection
	{
		public Rectangle OffCenter { get; set; }
		public float Near { get; set; }
		public float Far { get; set; }
		public HandDirection HandDirection { get; set; }

		public PerspectiveOffCenterProjection ( int left, int top, int right, int bottom, float near, float far )
		{
			OffCenter = new Rectangle ( new Vector2 ( left, top ),
				new Vector2 ( right - left, bottom - top ) );
			Near = near;
			Far = far;
		}

		public PerspectiveOffCenterProjection ( int width, int height, float near, float far )
		{
			OffCenter = new Rectangle ( new Vector2 (), new Vector2 ( width, height ) );
			Near = near;
			Far = far;
		}

		public Matrix4x4 Matrix
		{
			get
			{
				Func<float, float, float, float, float, float, Matrix4x4> perspOffCenter;

				if ( HandDirection == HandDirection.RightHand ) perspOffCenter = CommonTransform.PerspectiveOffCenterRH;
				else perspOffCenter = CommonTransform.PerspectiveOffCenterLH;

				return perspOffCenter ( OffCenter.Position.X,
					OffCenter.Position.X + OffCenter.Size.X, OffCenter.Position.Y + OffCenter.Size.Y, OffCenter.Position.Y, Near, Far );
			}
		}
	}
}
