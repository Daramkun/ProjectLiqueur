using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public class View : ITransform
	{
		public Vector3 Position { get; set; }
		public Vector3 Target { get; set; }
		public Vector3 UpVector { get; set; }
		public HandDirection HandDirection { get; set; }

		public View ( Vector3 position, Vector3 target, Vector3 upVector )
		{
			Position = position;
			Target = target;
			UpVector = upVector;
		}

		public Matrix4x4 Matrix
		{
			get
			{
				Func<Vector3, Vector3, Vector3, Matrix4x4> lookAt;

				if ( HandDirection == HandDirection.RightHand ) lookAt = CommonTransform.LookAtRH;
				else lookAt = CommonTransform.LookAtLH;

				return lookAt ( Position, Target, UpVector );
			}
		}
	}
}
