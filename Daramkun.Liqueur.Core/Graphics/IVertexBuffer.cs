using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	public enum FlexibleVertexFormat
	{
		PositionXY = 1 << 0,
		PositionXYZ = 1 << 1,

		Diffuse = 1 << 2,

		Normal = 1 << 3,

		TextureUV1 = 1 << 4,
		TextureUV2 = 1 << 5,
		TextureUV3 = 1 << 6,
		TextureUV4 = 1 << 7,
	}

	public interface IVertexBuffer<T> : IDisposable where T : struct
	{
		int Length { get; }
		int TotalBytesize { get; }

		object Handle { get; }

		FlexibleVertexFormat FVF { get; }

		T [] Vertices { get; set; }
	}
}
