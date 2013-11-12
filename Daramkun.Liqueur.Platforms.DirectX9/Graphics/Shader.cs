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
			: this ( graphicsDevice, SharpDX.Direct3D9.ShaderBytecode.Compile ( GetShaderSource ( stream ),
			GetShaderEntryPoint ( shaderType ), GetShaderProfile ( shaderType ), SharpDX.Direct3D9.ShaderFlags.None ),
			shaderType )
		{

		}

		private static string GetShaderProfile ( ShaderType shaderType )
		{
			switch ( shaderType )
			{
				case ShaderType.VertexShader: return "vs_2_0";
				case ShaderType.PixelShader: return "ps_2_0";
				default: throw new ArgumentException ();
			}
		}

		private static string GetShaderSource ( Stream stream )
		{
			using ( StreamReader reader = new StreamReader ( stream ) )
			{
				return reader.ReadToEnd ();
			}
		}

		private static string GetShaderEntryPoint ( ShaderType shaderType )
		{
			switch ( shaderType )
			{
				case ShaderType.VertexShader: return "vs_main";
				case ShaderType.PixelShader: return "ps_main";
				default: throw new ArgumentException ();
			}
		}

		public Shader ( IGraphicsDevice graphicsDevice, string shaderCode, Graphics.ShaderType shaderType )
			: this ( graphicsDevice, SharpDX.Direct3D9.ShaderBytecode.Compile ( shaderCode,
			GetShaderEntryPoint ( shaderType ), GetShaderProfile ( shaderType ), SharpDX.Direct3D9.ShaderFlags.None ), shaderType )
		{

		}

		public void Dispose ()
		{
			( Handle as IDisposable ).Dispose ();
		}
	}
}
