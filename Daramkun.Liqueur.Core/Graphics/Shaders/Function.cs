using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics.Shaders
{
	public struct Function
	{
		public string Name { get; set; }
		public DataType ReturnType { get; set; }
		public List<Variable> Parameters { get; set; }
		public List<IExpression> Operations { get; set; }
	}
}
