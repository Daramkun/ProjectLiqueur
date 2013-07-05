using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Math.Transforms
{
	public interface IProjection : ITransform
	{
		float Near { get; set; }
		float Far { get; set; }
	}
}
