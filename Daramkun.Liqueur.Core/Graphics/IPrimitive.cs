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

	public interface IPrimitive<T> : IDisposable where T : IFlexibleVertex
	{
		FlexibleVertexArray<T> Vertices { get; }
		int [] Indices { get; }

		PrimitiveType PrimitiveType { get; set; }
		int PrimitiveCount { get; set; }
		IEffect Effect { get; set; }
		ITexture2D Texture { get; set; }
		ITexture2D SubTexture { get; set; }
	}
}
