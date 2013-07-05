using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics.Shaders
{
	public interface IShaderParser
	{
		Shader [] Parse ( Stream stream );
	}
}
