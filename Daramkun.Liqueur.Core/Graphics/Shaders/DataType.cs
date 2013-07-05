using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics.Shaders
{
	public enum DataType
	{
		Void,

		Boolean,
		Integer,
		UnsignedInteger,
		FloatingPoint,

		Vector2,
		Vector3,
		Vector4,

		Matrix2,
		Matrix3,
		Matrix4,

		Sampler1D,
		Sampler2D,
		Sampler3D,
		SamplerCube,
	}
}
