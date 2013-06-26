using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	public enum StencilFunction
	{
		Never,
		Less,
		Equal,
		LessEqual,
		Greater,
		NotEqual,
		GreaterEqual,
		Always,
	}

	public enum StencilOperation
	{
		Keep,
		Zero,
		Replace,
		IncreaseSAT,
		DecreaseSAT,
		Invert,
		Increase,
		Decrease,
	}

	public struct Stencil
	{
		public StencilFunction Function { get; set; }
		public int Reference { get; set; }
		public int Mask { get; set; }
		public StencilOperation ZFail { get; set; }
		public StencilOperation Fail { get; set; }
		public StencilOperation Pass { get; set; }
	}
}
