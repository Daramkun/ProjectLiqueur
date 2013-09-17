using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SharpDX.Direct3D11;

namespace Daramkun.Liqueur.Graphics
{
	class Shader : IShader
	{
		IDisposable shader;

		public ShaderType ShaderType { get { throw new NotImplementedException (); } }
		public ShaderOption Option { get; set; }
		public object Handle { get { return shader; } }

		public Shader ( IGraphicsDevice graphicsDevice, string code, ShaderType shaderType )
		{
			byte [] shaderData = SharpDX.D3DCompiler.ShaderBytecode.Compile ( Encoding.UTF8.GetBytes ( code ), 
				(shaderType == ShaderType.VertexShader) ? "vs_5_0" : "ps_5_0" );
			switch ( shaderType )
			{
				case ShaderType.VertexShader: shader = new VertexShader ( graphicsDevice.Handle as Device, shaderData ); break;
				case ShaderType.PixelShader: shader = new PixelShader ( graphicsDevice.Handle as Device, shaderData ); break;
			}
		}

		public Shader ( IGraphicsDevice graphicsDevice, Stream stream, ShaderType shaderType )
			: this ( graphicsDevice, new StreamReader ( stream ).ReadToEnd (), shaderType )
		{ }

		~Shader ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				shader.Dispose ();
				shader = null;
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}
	}
}
