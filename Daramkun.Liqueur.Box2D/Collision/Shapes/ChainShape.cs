using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Mathematics;
using Daramkun.Liqueur.Box2D.Common;

namespace Daramkun.Liqueur.Box2D.Collision.Shapes
{
	public class ChainShape : Shape
	{
		Vector2 [] m_vertices;
		
		Vector2 m_prevVertex, m_nextVertex;
		bool m_hasPrevVertex, m_hasNextVertex;

		public ChainShape ()
		{
			ShapeType = ShapeType.Chain;
			m_radius = Settings.PolygonRadius;
		}

		public void CreateLoop ( Vector2 [] vertices )
		{

		}

		public void CreateChain ( Vector2 [] vertices )
		{

		}

		public void SetPreviousVertex ( Vector2 prevVertex )
		{

		}

		public void SetNextVertex ( Vector2 nextVertex )
		{

		}
	}
}
