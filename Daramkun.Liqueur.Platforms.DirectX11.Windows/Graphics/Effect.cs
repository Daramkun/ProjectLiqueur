using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Graphics
{
	class Effect : IEffect
	{
		IGraphicsDevice graphicsDevice;
		List<IShader> list;

		public object Handle { get { return list; } }

		public Effect ( IGraphicsDevice graphicsDevice, params IShader [] shaders )
		{
			this.graphicsDevice = graphicsDevice;
			list = new List<IShader> ( shaders );
		}

		public void Dispose ()
		{

		}

		public void Dispatch ( Action<IEffect> dispatchEvent )
		{
			foreach(IShader shader in list)
				switch ( shader.ShaderType )
				{
					case ShaderType.VertexShader:
						( graphicsDevice.Handle as SharpDX.Direct3D11.Device ).ImmediateContext.VertexShader.Set (
						shader.Handle as SharpDX.Direct3D11.VertexShader );
						break;
					case ShaderType.PixelShader: 
						( graphicsDevice.Handle as SharpDX.Direct3D11.Device ).ImmediateContext.PixelShader.Set (
						shader.Handle as SharpDX.Direct3D11.PixelShader );
						break;
				}

			dispatchEvent ( this );


		}

		public T GetArgument<T> ( string parameter )
		{
			throw new NotImplementedException ();
		}

		public void SetArgument<T> ( string parameter, T argument )
		{
			throw new NotImplementedException ();
		}

		public void SetTextures ( params TextureArgument [] textures )
		{
			throw new NotImplementedException ();
		}
	}
}
