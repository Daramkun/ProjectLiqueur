using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Math
{
	public enum ProjectionMode
	{
		Perspective,
		Orthographic,
	}

	public struct Projection
	{
		Matrix4x4 matrix;

		public ProjectionMode ProjectionMode { get; set; }
		public Matrix4x4 Matrix { get { return matrix; } }

		public static Projection CreateOrthographic ( float width, float height, float nearPlane, float farPlane )
		{
			Projection proj = new Projection ();
			proj.ProjectionMode = ProjectionMode.Orthographic;
			proj.matrix.M11 = 2 / width;
			proj.matrix.M22 = 2 / height;
			proj.matrix.M33 = 1 / ( nearPlane - farPlane );
			proj.matrix.M43 = nearPlane / ( nearPlane - farPlane );
			proj.matrix.M44 = 1;
			return proj;
		}

		public static Projection CreateOrthographicOffCenter ( float left, float right, float bottom, float top, float nearPlane, float farPlane )
		{
			Projection proj = new Projection ();
			proj.ProjectionMode = ProjectionMode.Orthographic;
			proj.matrix.M11 = 2 / ( right - left );
			proj.matrix.M22 = 2 / ( bottom - top );
			proj.matrix.M33 = 1 / ( nearPlane - farPlane );
			proj.matrix.M41 = ( left + right ) / ( left - right );
			proj.matrix.M42 = ( top + bottom ) / ( bottom - top );
			proj.matrix.M43 = nearPlane / ( nearPlane - farPlane );
			proj.matrix.M44 = 1;
			return proj;
		}

		public static Projection CreatePerspective ( float width, float height, float nearPlaneDistance, float farPlaneDistance )
		{
			Projection proj = new Projection ();
			proj.ProjectionMode = ProjectionMode.Perspective;
			proj.matrix.M11 = ( 2 * nearPlaneDistance ) / width;
			proj.matrix.M22 = ( 2 * nearPlaneDistance ) / height;
			proj.matrix.M33 = farPlaneDistance / ( nearPlaneDistance - farPlaneDistance );
			proj.matrix.M34 = -1f;
			proj.matrix.M43 = ( nearPlaneDistance * farPlaneDistance ) / ( nearPlaneDistance - farPlaneDistance );
			return proj;
		}

		public static Projection CreatePerspectiveFieldOfView ( float fieldOfView, float aspectRatio, float nearPlaneDistance, float farPlaneDistance )
		{
			Projection proj = new Projection ();
			proj.ProjectionMode = ProjectionMode.Perspective;
			float num = 1 / ( float ) System.Math.Tan ( fieldOfView * 0.5f );
			proj.matrix.M11 = num / aspectRatio;
			proj.matrix.M22 = num;
			proj.matrix.M33 = farPlaneDistance / ( nearPlaneDistance - farPlaneDistance );
			proj.matrix.M34 = -1f;
			proj.matrix.M43 = ( nearPlaneDistance * farPlaneDistance ) / ( nearPlaneDistance - farPlaneDistance );
			return proj;
		}

		public static Projection CreatePerspectiveOffCenter ( float left, float right, float bottom, float top, float nearPlaneDistance, float farPlaneDistance )
		{
			Projection proj = new Projection ();
			proj.ProjectionMode = ProjectionMode.Perspective;
			proj.matrix.M11 = ( 2 * nearPlaneDistance ) / ( right - left );
			proj.matrix.M22 = ( 2 * nearPlaneDistance ) / ( top - bottom );
			proj.matrix.M31 = ( left + right ) / ( right - left );
			proj.matrix.M32 = ( top + bottom ) / ( top - bottom );
			proj.matrix.M33 = farPlaneDistance / ( nearPlaneDistance - farPlaneDistance );
			proj.matrix.M34 = -1;
			proj.matrix.M43 = ( nearPlaneDistance * farPlaneDistance ) / ( nearPlaneDistance - farPlaneDistance );
			return proj;
		}
	}
}
