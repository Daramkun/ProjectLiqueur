using Daramkun.Liqueur.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	public struct Transform2
	{
		Vector2 translate;
		Vector2 scaleCenter, scale;
		float rotation;
		Vector2 rotationCenter;

		public Vector2 Translate { get { return translate; } set { translate = value; } }
		public Vector2 ScaleCenter { get { return scaleCenter; } set { scaleCenter = value; } }
		public Vector2 Scale { get { return scale; } set { scale = value; } }
		public float Rotation { get { return rotation; } set { rotation = value; } }
		public Vector2 RotationCenter { get { return rotationCenter; } set { rotationCenter = value; } }

		public static Transform2 Identity
		{
			get { return new Transform2 () { Scale = new Vector2 ( 1, 1 ) }; }
		}

		public Transform2 ( Vector2 translate )
		{
			this.translate = translate;
			this.scaleCenter = new Vector2 ();
			this.scale = new Vector2 ( 1, 1 );
			this.rotation = 0;
			this.rotationCenter = new Vector2 ();
		}

		public Transform2 ( Vector2 scaleCenter, Vector2 scale )
		{
			this.translate = new Vector2 ();
			this.scaleCenter = scaleCenter;
			this.scale = scale;
			this.rotation = 0;
			this.rotationCenter = new Vector2 ();
		}

		public Transform2 ( Vector2 translate, Vector2 scaleCenter, Vector2 scale )
		{
			this.translate = translate;
			this.scaleCenter = scaleCenter;
			this.scale = scale;
			this.rotation = 0;
			this.rotationCenter = new Vector2 ();
		}

		public Transform2 ( float rotation, Vector2 rotationCenter )
		{
			this.translate = new Vector2 ();
			this.scaleCenter = new Vector2 ();
			this.scale = new Vector2 ( 1, 1 );
			this.rotation = rotation;
			this.rotationCenter = rotationCenter;
		}

		public Transform2 ( Vector2 translate, float rotation, Vector2 rotationCenter )
		{
			this.translate = translate;
			this.scaleCenter = new Vector2 ();
			this.scale = new Vector2 ( 1, 1 );
			this.rotation = rotation;
			this.rotationCenter = rotationCenter;
		}

		public Transform2 ( Vector2 scaleCenter, Vector2 scale, float rotation, Vector2 rotationCenter )
		{
			this.translate = new Vector2 ();
			this.scaleCenter = scaleCenter;
			this.scale = scale;
			this.rotation = rotation;
			this.rotationCenter = rotationCenter;
		}

		public Transform2 ( Vector2 translate, Vector2 scaleCenter, Vector2 scale, float rotation, Vector2 rotationCenter )
		{
			this.translate = translate;
			this.scaleCenter = scaleCenter;
			this.scale = scale;
			this.rotation = rotation;
			this.rotationCenter = rotationCenter;
		}

		public static Transform2 operator + ( Transform2 v1, Transform2 v2 )
		{
			return new Transform2 ( v1.translate + v2.translate, v1.scaleCenter + v2.scaleCenter,
				v1.scale * v2.scale, v1.rotation + v2.rotation, v1.rotationCenter + v2.rotationCenter );
		}

		public static Transform2 operator - ( Transform2 v1, Transform2 v2 )
		{
			return new Transform2 ( v1.translate - v2.translate, v1.scaleCenter - v2.scaleCenter,
				v1.scale / v2.scale, v1.rotation - v2.rotation, v1.rotationCenter - v2.rotationCenter );
		}
	}
}
