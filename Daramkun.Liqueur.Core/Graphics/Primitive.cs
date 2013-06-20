using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Graphics.Vertices;
using Daramkun.Liqueur.Math;

namespace Daramkun.Liqueur.Graphics
{
	public enum PrimitiveType
	{
		PointList,
		LineList,
		LineStrip,
		TriangleList,
		TriangleStrip,
		TriangleFan,
	}

	public struct Primitive<T> where T : IFlexibleVertex
	{
		public T [] Vertices { get; set; }
		public int [] Indices { get; set; }
		public bool IsIndexPrimitive { get; set; }
		public PrimitiveType PrimitiveType { get; set; }
	}
}
