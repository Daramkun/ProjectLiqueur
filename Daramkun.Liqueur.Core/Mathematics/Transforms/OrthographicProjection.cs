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
		public HandDirection HandDirection { get; set; }

		public OrthographicProjection ( float width, float height, float near, float far )
		{
			Area = new Rectangle ( new Vector2 (), new Vector2 ( width, height ) );
			Near = near;
			Far = far;
		}

		public OrthographicProjection ( float left, float right, float bottom, float top, float near, float far )
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
				
				Func<float, float, float, float, float, float, Matrix4x4> orthoOffCenter;

				if ( HandDirection == HandDirection.RightHand ) orthoOffCenter = CommonTransform.OrthographicOffCenterRH;
				else orthoOffCenter = CommonTransform.OrthographicOffCenterLH;

				return orthoOffCenter ( left, right, bottom, top, Near, Far );
			}
		}
	}
}
