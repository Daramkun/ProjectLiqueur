using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Box2D.Common
{
	public static class Settings
	{
		public static int MaxManifoldPoints = 2;

		public static int MaxPolygonVertices = 8;
		public static float AABBExtension = 0.1f;
		public static float AABBMultiplier = 2.0f;
		public static float LinearSlop = 0.005f;
		public static float AngularSlop = ( 2.0f / 180.0f * ( float ) Math.PI );
		public static float PolygonRadius = ( 2.0f * LinearSlop );
		public static int MaxSubSteps = 8;

		public static int MaxTOIContacts = 32;
		public static float VelocityThreshold = 1.0f;
		public static float MaxLinearCorrection = 0.2f;
		public static float MaxAngularCorrection = ( 8.0f / 180.0f * ( float ) Math.PI );
		public static float MaxTranslation = 2.0f;
		public static float MaxTranslationSquared = ( MaxTranslation * MaxTranslation );
		public static float MaxRotation = ( 0.5f * ( float ) Math.PI );
		public static float MaxRotationSquared = ( MaxRotation * MaxRotation );
		public static float Baumgarte = 0.2f;
		public static float ToiBaugarte = 0.75f;

		public static Version Version { get { return new Version ( 2, 2, 1 ); } }
	}
}
