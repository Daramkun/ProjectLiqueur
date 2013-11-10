using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramkun.Liqueur.Graphics
{
	class Effect : IEffect
	{
		IGraphicsDevice graphicsDevice;
		IShader vertexShader;
		IShader pixelShader;

		public object Handle
		{
			get { throw new NotImplementedException (); }
		}

		public Effect ( IGraphicsDevice graphicsDevice, params IShader [] shaders )
		{
			this.graphicsDevice = graphicsDevice;

			foreach ( IShader shader in shaders )
			{
				switch ( shader.ShaderType )
				{
					case ShaderType.VertexShader: vertexShader = shader; break;
					case ShaderType.PixelShader: pixelShader = shader; break;
				}
			}
		}

		public void Dispose ()
		{

		}

		public void Dispatch ( Action<IEffect> dispatchEvent )
		{
			SharpDX.Direct3D9.Device device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;

			var oldVertexShader = device.VertexShader;
			var oldPixelShader = device.PixelShader;

			device.VertexShader = vertexShader.Handle as SharpDX.Direct3D9.VertexShader;
			device.PixelShader = pixelShader.Handle as SharpDX.Direct3D9.PixelShader;

			dispatchEvent ( this );

			device.VertexShader = oldVertexShader;
			device.PixelShader = oldPixelShader;
		}

		public T GetArgument<T> ( string parameter )
		{
			SharpDX.Direct3D9.Device device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			SharpDX.Direct3D9.EffectHandle handle = ( vertexShader as Shader ).constantTable.GetConstantByName ( null, parameter );
			throw new NotImplementedException ();
		}

		public void SetArgument<T> ( string parameter, T argument )
		{
			SharpDX.Direct3D9.Device device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			SharpDX.Direct3D9.EffectHandle handle = ( vertexShader as Shader ).constantTable.GetConstantByName ( null, parameter );

		}

		public void SetTexture ( TextureArgument texture )
		{
			SharpDX.Direct3D9.Device device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			SharpDX.Direct3D9.EffectHandle handle = ( pixelShader as Shader ).constantTable.GetConstantByName ( null, texture.Uniform );
		}

		public void SetTextures ( params TextureArgument [] textures )
		{
			foreach ( TextureArgument texture in textures )
				SetTexture ( texture );
		}
	}
}
