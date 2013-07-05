using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics.Shaders
{
	public enum VariableUsecase
	{
		None,

		Position,
		Normal,
		Color,
		TextureCoord1,
		TextureCoord2,
	}

	public struct Variable
	{
		public string Name { get; set; }
		public DataType VariableType { get; set; }
		public VariableUsecase Usecase { get; set; }
	}
}
