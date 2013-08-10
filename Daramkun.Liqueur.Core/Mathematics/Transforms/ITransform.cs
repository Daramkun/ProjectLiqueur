using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Mathematics.Transforms
{
	public interface ITransform
	{
		Matrix4x4 Matrix { get; }
	}
}
