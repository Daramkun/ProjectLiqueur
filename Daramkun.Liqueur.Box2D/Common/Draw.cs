using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Box2D.Common
{
	public enum DrawFlag
	{
		ShapeBit = 0x0001,
		JointBit = 0x0002,
		AABBBit = 0x0004,
		PairBit = 0x0008,
		CenterOfMassBit = 0x0010
	}

	public abstract class Draw
	{
		public DrawFlag Flag { get; set; }

		public void SetFlag ( DrawFlag flag ) { Flag = flag; }
		public void AppendFlag ( DrawFlag flag ) { Flag |= flag; }
		public void ClearFlag ( DrawFlag flag ) { Flag &= flag; }

		public abstract void DrawPolygon ( Vector2 [] vertices, Color color );
		public abstract void DrawSolidPolygon ( Vector2 [] vertices, Color color );
		public abstract void DrawCircle ( Vector2 center, float radius, Color color );
		public abstract void DrawSolidCircle ( Vector2 center, float radius, Vector2 axis, Color color );
		public abstract void DrawSegment ( Vector2 p1, Vector2 p2, Color color );
		public abstract void DrawTransform ( Transform xf );
	}
}
