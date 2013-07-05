using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics.Shaders
{
	public interface IShaderComposer
	{
		string Compose ( Shader shader );
	}
}