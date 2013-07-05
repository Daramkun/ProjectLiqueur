using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics.Shaders
{
	public enum ParameterType
	{
		Input,
		Output,
		Uniform,
	}

	public struct Parameter
	{
		public string Name { get; set; }
		public ParameterType ParameterType { get; set; }
		public List<Variable> Variables { get; set; }
	}
}
