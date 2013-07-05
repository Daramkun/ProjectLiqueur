using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics.Shaders
{
	public enum ShaderType
	{
		VertexShader,
		PixelShader,
	}

	public struct Shader
	{
		public string Name { get; set; }
		public ShaderType ShaderType { get; set; }

		public List<Parameter> Parameters { get; set; }
		public List<Function> Functions { get; set; }
	}
}
