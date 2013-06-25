using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Math
{
	public interface ITransform
	{
		Matrix4x4 Matrix { get; }
	}

	public interface ITransform<T1, T2> : ITransform
	{
		T1 Translate { get; set; }
		T1 Scale { get; set; }
		T1 ScaleCenter { get; set; }
		T2 Rotation { get; set; }
		T1 RotationCenter { get; set; }
	}
}
