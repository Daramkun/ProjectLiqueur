using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Mathematics.Transforms;

namespace Daramkun.Liqueur.Engine.Horn.Common
{
	public class Camera
	{
		Vector3 position;
		Vector3 target;
		Vector3 upVector;

		Vector3 movement;
		float yaw, pitch, roll;

		View view;

		public HandDirection HandDirection
		{
			get { return view.HandDirection; }
			set { view.HandDirection = value; }
		}

		public Camera () : this ( new Vector3 () ) { }

		public Camera ( Vector3 position )
		{
			this.position = position;
			view = new View ( new Vector3 (), new Vector3 (), new Vector3 () );
		}

		public void Strafe ( float unit )
		{
			movement.X -= unit;
		}

		public void Walk ( float unit )
		{
			movement.Z -= unit;
		}

		public void Yaw ( float unit )
		{
			yaw += unit;
			if ( yaw > Math.PI * 2 ) yaw -= ( float ) Math.PI * 2;
			if ( yaw < -Math.PI * 2 ) yaw += ( float ) Math.PI * 2;
		}

		public void Pitch ( float unit )
		{
			pitch += unit;
			if ( pitch > Math.PI * 2 ) pitch -= ( float ) Math.PI * 2;
			if ( pitch < -Math.PI * 2 ) pitch += ( float ) Math.PI * 2;
		}

		public void Roll ( float unit )
		{
			roll += unit;
			if ( roll > Math.PI * 2 ) roll -= ( float ) Math.PI * 2;
			if ( roll < -Math.PI * 2 ) roll += ( float ) Math.PI * 2;
		}

		public Matrix4x4 Matrix
		{
			get
			{
				Matrix4x4 tempMatrix = CommonTransform.FromYawPitchRoll ( yaw, 0, 0 );
				
				Vector3 transTemp = CommonTransform.TransformCoord ( movement, tempMatrix );
				position += transTemp;

				tempMatrix = CommonTransform.FromYawPitchRoll ( yaw, pitch, roll );
				target = CommonTransform.TransformCoord ( new Vector3 ( 0, 0, -1 ), tempMatrix );
				upVector = CommonTransform.TransformCoord ( new Vector3 ( 0, 1, 0 ), tempMatrix );

				view.Position = position;
				view.Target = target;
				view.UpVector = upVector;

				return view.Matrix;
			}
		}
	}
}
