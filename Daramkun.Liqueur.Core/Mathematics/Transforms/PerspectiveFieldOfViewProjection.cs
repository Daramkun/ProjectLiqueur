using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public class PerspectiveFieldOfViewProjection : IPerspectiveProjection
	{
		public float FieldOfView { get; set; }
		public float AspectRatio { get; set; }
		public float Near { get; set; }
		public float Far { get; set; }
		public HandDirection HandDirection { get; set; }

		public PerspectiveFieldOfViewProjection ( float fieldOfView, float aspectRatio, float near, float far )
			//: this ()
		{
			FieldOfView = fieldOfView;
			AspectRatio = aspectRatio;
			Near = near;
			Far = far;
		}

		public Matrix4x4 Matrix
		{
			get
			{
				Func<float, float, float, float, Matrix4x4> perspFov;

				if ( HandDirection == HandDirection.RightHand ) perspFov = CommonTransform.PerspectiveFieldOfViewRH;
				else perspFov = CommonTransform.PerspectiveFieldOfViewLH;

				return perspFov ( FieldOfView, AspectRatio, Near, Far );
			}
		}
	}
}
