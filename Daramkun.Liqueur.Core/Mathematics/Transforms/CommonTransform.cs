using System;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public static class CommonTransform
	{
		public static Matrix4x4 LookAtLH ( Vector3 eye, Vector3 at, Vector3 up )
		{
			Vector3 zaxis = Vector3.Normalize ( at - eye );
			Vector3 xaxis = Vector3.Normalize ( Vector3.Cross ( up, zaxis ) );
			Vector3 yaxis = Vector3.Cross ( zaxis, xaxis );
			return new Matrix4x4
			(
				xaxis.X, yaxis.X, zaxis.X, 0,
				xaxis.Y, yaxis.Y, zaxis.Y, 0,
				xaxis.Z, yaxis.Z, zaxis.Z, 0,
				-Vector3.Dot ( xaxis, eye ), -Vector3.Dot ( yaxis, eye ), -Vector3.Dot ( zaxis, eye ), 1
			);
		}

		public static Matrix4x4 LookAtRH ( Vector3 eye, Vector3 at, Vector3 up )
		{
			Vector3 zaxis = Vector3.Normalize ( eye - at );
			Vector3 xaxis = Vector3.Normalize ( Vector3.Cross ( up, zaxis ) );
			Vector3 yaxis = Vector3.Cross ( zaxis, xaxis );
			return new Matrix4x4
			(
				xaxis.X, yaxis.X, zaxis.X, 0,
				xaxis.Y, yaxis.Y, zaxis.Y, 0,
				xaxis.Z, yaxis.Z, zaxis.Z, 0,
				-Vector3.Dot ( xaxis, eye ), -Vector3.Dot ( yaxis, eye ), -Vector3.Dot ( zaxis, eye ), 1
			);
		}

		public static Matrix4x4 OrthographicLH ( float w, float h, float zn, float zf )
		{
			return new Matrix4x4
			(
				2 / w, 0, 0, 0,
				0, 2 / h, 0, 0,
				0, 0, 1 / ( zf - zn ), 0,
				0, 0, -zn / ( zf - zn ), 0
			);
		}

		public static Matrix4x4 OrthographicRH ( float w, float h, float zn, float zf )
		{
			return new Matrix4x4
			(
				2 / w, 0, 0, 0,
				0, 2 / h, 0, 0,
				0, 0, 1 / ( zn - zf ), 0,
				0, 0, zn / ( zn - zf ), 0
			);
		}

		public static Matrix4x4 OrthographicOffCenterLH ( float l, float r, float b, float t, float zn, float zf )
		{
			return new Matrix4x4
			(
				2 / ( r - l ), 0, 0, 0,
				0, 2 / ( t - b ), 0, 0,
				0, 0, 1 / ( zf - zn ), 0,
				( l + r ) / ( l - r ), ( t + b ) / ( b - t ), -zn / ( zf - zn ), 1
			);
		}

		public static Matrix4x4 OrthographicOffCenterRH ( float l, float r, float b, float t, float zn, float zf )
		{
			return new Matrix4x4
			(
				2 / ( r - l ), 0, 0, 0,
				0, 2 / ( t - b ), 0, 0,
				0, 0, 1 / ( zn - zf ), 0,
				( l + r ) / ( l - r ), ( t + b ) / ( b - t ), zn / ( zf - zn ), 1
			);
		}

		public static Matrix4x4 PerspectiveFieldOfViewLH ( float fov, float aspect, float zn, float zf )
		{
			float yScale = ( float ) ( Math.Cos ( fov / 2 ) / Math.Sin ( fov / 2 ) ), xScale = yScale / aspect;
			return new Matrix4x4
			(
				xScale, 0, 0, 0,
				0, yScale, 0, 0,
				0, 0, zf / ( zf - zn ), 1,
				0, 0, -zn * zf / ( zf - zn ), 0
			);
		}

		public static Matrix4x4 PerspectiveFieldOfViewRH ( float fov, float aspect, float zn, float zf )
		{
			float yScale = ( float ) ( Math.Cos ( fov / 2 ) / Math.Sin ( fov / 2 ) ), xScale = yScale / aspect;
			return new Matrix4x4
			(
				xScale, 0, 0, 0,
				0, yScale, 0, 0,
				0, 0, zf / ( zn - zf ), -1,
				0, 0, zn * zf / ( zn - zf ), 0
			);
		}

		public static Matrix4x4 PerspectiveLH ( float w, float h, float zn, float zf )
		{
			return new Matrix4x4
			(
				2 * zn / w, 0, 0, 0,
				0, 2 * zn / h, 0, 0,
				0, 0, zf / ( zf - zn ), 1,
				0, 0, zn * zf / ( zn - zf ), 0
			);
		}

		public static Matrix4x4 PerspectiveRH ( float w, float h, float zn, float zf )
		{
			return new Matrix4x4
			(
				2 * zn / w, 0, 0, 0,
				0, 2 * zn / h, 0, 0,
				0, 0, zf / ( zn - zf ), 1,
				0, 0, zn * zf / ( zn - zf ), 0
			);
		}

		public static Matrix4x4 PerspectiveOffCenterLH ( float l, float r, float b, float t, float zn, float zf )
		{
			return new Matrix4x4
			(
				2 * zn / ( r - l ), 0, 0, 0,
				0, 2 * zn / ( t - b ), 0, 0,
				( l + r ) / ( l - r ), ( t + b ) / ( b - t ), zf / ( zf - zn ), 1,
				0, 0, zn * zf / ( zn - zf ), 0
			);
		}

		public static Matrix4x4 PerspectiveOffCenterRH ( float l, float r, float b, float t, float zn, float zf )
		{
			return new Matrix4x4
			(
				2 * zn / ( r - l ), 0, 0, 0,
				0, 2 * zn / ( t - b ), 0, 0,
				( l + r ) / ( l - r ), ( t + b ) / ( t - b ), zf / ( zn - zf ), 1,
				0, 0, zn * zf / ( zn - zf ), 0
			);
		}

		public static Matrix4x4 RotationX ( float angle )
		{
			return new Matrix4x4
			(
				1, 0, 0, 0,
				0, ( float ) Math.Cos ( angle ), ( float ) Math.Sin ( angle ), 0,
				0, -( float ) Math.Sin ( angle ), ( float ) Math.Cos ( angle ), 0,
				0, 0, 0, 1
			);
		}

		public static Matrix4x4 RotationY ( float angle )
		{
			return new Matrix4x4
			(
				( float ) Math.Cos ( angle ), 0, -( float ) Math.Sin ( angle ), 0,
				0, 1, 0, 0,
				( float ) Math.Sin ( angle ), 0, ( float ) Math.Cos ( angle ), 0,
				0, 0, 0, 1
			);
		}

		public static Matrix4x4 RotationZ ( float angle )
		{
			return new Matrix4x4
			(
				( float ) Math.Cos ( angle ), ( float ) Math.Sin ( angle ), 0, 0,
				-( float ) Math.Sin ( angle ), ( float ) Math.Cos ( angle ), 0, 0,
				0, 0, 1, 0,
				0, 0, 0, 1
			);
		}

		public static Matrix4x4 RotationXYZ ( float xa, float ya, float za )
		{
			return RotationX ( xa ) * RotationY ( ya ) * RotationZ ( za );
		}

		public static Matrix4x4 RotationXYZ ( Vector3 xyza ) { return RotationXYZ ( xyza.X, xyza.Y, xyza.Z ); }

		public static Matrix4x4 Scale ( float sx, float sy, float sz )
		{
			return new Matrix4x4
			(
				sx, 0, 0, 0,
				0, sy, 0, 0,
				0, 0, sz, 0,
				0, 0, 0, 1
			);
		}

		public static Matrix4x4 Scale ( Vector3 s ) { return Scale ( s.X, s.Y, s.Z ); }

		public static Matrix4x4 Translate ( float x, float y, float z )
		{
			return new Matrix4x4
			(
				1, 0, 0, 0,
				0, 1, 0, 0,
				0, 0, 1, 0,
				x, y, z, 1
			);
		}

		public static Matrix4x4 Translate ( Vector3 p ) { return Translate ( p.X, p.Y, p.Z ); }

		public static Matrix4x4 FromAxisAngle ( float x, float y, float z, float angle )
		{
			float num2 = ( float ) Math.Sin ( angle );
			float num = ( float ) Math.Cos ( angle );
			float num11 = x * x;
			float num10 = y * y;
			float num9 = z * z;
			float num8 = x * y;
			float num7 = x * z;
			float num6 = y * z;

			return new Matrix4x4
			(
				num11 + ( num * ( 1 - num11 ) ), ( num8 - ( num * num8 ) ) + ( num2 * z ), ( num7 - ( num * num7 ) ) - ( num2 * y ), 0,
				( num8 - ( num * num8 ) ) - ( num2 * z ), num10 + ( num * ( 1f - num10 ) ), ( num6 - ( num * num6 ) ) + ( num2 * x ), 0,
				( num7 - ( num * num7 ) ) + ( num2 * y ), ( num6 - ( num * num6 ) ) - ( num2 * x ), num9 + ( num * ( 1f - num9 ) ), 0,
				0, 0, 0, 1
			);
		}

		public static Matrix4x4 FromAxisAngle ( Vector3 n, float angle ) { return FromAxisAngle ( n.X, n.Y, n.Z, angle ); }

		public static Matrix4x4 FromYawPitchRoll ( float yaw, float pitch, float roll )
		{
			return Quaternion.FromYawPitchRoll ( yaw, pitch, roll ).ToMatrix4x4 ();
		}

		public static Matrix4x4 FromYawPitchRoll ( Vector3 ypr ) { return FromYawPitchRoll ( ypr.X, ypr.Y, ypr.Z ); }

		public static Vector3 TransformCoord ( Vector3 pos, Matrix4x4 matrix )
		{
			return new Vector3 (
				( pos.X * matrix.M11 ) + ( pos.Y * matrix.M21 ) + ( pos.Z * matrix.M31 ) + matrix.M41,
				( pos.X * matrix.M12 ) + ( pos.Y * matrix.M22 ) + ( pos.Z * matrix.M32 ) + matrix.M42,
				( pos.X * matrix.M13 ) + ( pos.Y * matrix.M23 ) + ( pos.Z * matrix.M33 ) + matrix.M43
			);
		}
	}
}

