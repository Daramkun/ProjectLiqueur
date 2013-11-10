using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramkun.Liqueur.Graphics
{
	class Shader : IShader
	{
		public ShaderType ShaderType { get; private set; }
		public ShaderOption Option { get; set; }
		public object Handle { get; private set; }

		internal SharpDX.Direct3D9.ConstantTable constantTable;

		private Shader ( IGraphicsDevice graphicsDevice, SharpDX.Direct3D9.ShaderBytecode function, Graphics.ShaderType shaderType )
		{
			ShaderType = shaderType;
			constantTable = function.ConstantTable;
			switch ( ShaderType )
			{
				case Graphics.ShaderType.VertexShader:
					Handle = new SharpDX.Direct3D9.VertexShader ( graphicsDevice.Handle as SharpDX.Direct3D9.Device, function );
					break;
				case Graphics.ShaderType.PixelShader:
					Handle = new SharpDX.Direct3D9.PixelShader ( graphicsDevice.Handle as SharpDX.Direct3D9.Device, function );
					break;
			}
		}

		public Shader ( IGraphicsDevice graphicsDevice, Stream stream, Graphics.ShaderType shaderType )
			: this ( graphicsDevice, new SharpDX.Direct3D9.ShaderBytecode ( stream ), shaderType )
		{

		}

		public Shader ( IGraphicsDevice graphicsDevice, string shaderCode, Graphics.ShaderType shaderType )
			: this ( graphicsDevice, new SharpDX.Direct3D9.ShaderBytecode ( Encoding.UTF8.GetBytes ( shaderCode ) ), shaderType )
		{

		}

		public void Dispose ()
		{
			( Handle as IDisposable ).Dispose ();
		}
	}
}
