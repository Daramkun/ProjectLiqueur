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

	public enum StencilOperator
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

	public struct StencilOperation
	{
		public StencilFunction Function { get; set; }
		public int Reference { get; set; }
		public int Mask { get; set; }
		public StencilOperator ZFail { get; set; }
		public StencilOperator Fail { get; set; }
		public StencilOperator Pass { get; set; }
	}
}
