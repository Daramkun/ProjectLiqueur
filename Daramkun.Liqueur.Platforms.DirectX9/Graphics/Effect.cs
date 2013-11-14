using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daramkun.Liqueur.Mathematics;

namespace Daramkun.Liqueur.Graphics
{
	class Effect : IEffect
	{
		IGraphicsDevice graphicsDevice;
		IShader vertexShader;
		IShader pixelShader;

		public object Handle
		{
			get { return new [] { vertexShader, pixelShader }; }
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
			pixelShader.Dispose ();
			vertexShader.Dispose ();
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

		public T GetArgument<T> ( string parameter ) where T : struct
		{
			var device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			var handle = ( vertexShader.Handle as SharpDX.Direct3D9.VertexShader ).Function.ConstantTable.GetConstantByName ( null, parameter );
			throw new NotImplementedException ();
		}

		public void SetArgument<T> ( string parameter, T argument ) where T : struct
		{
			var device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			var constantTable = ( vertexShader.Handle as SharpDX.Direct3D9.VertexShader ).Function.ConstantTable;
			var handle = constantTable.GetConstantByName ( null, parameter );
			constantTable.SetValue<T> ( device, handle, argument );
		}

		public void SetTexture ( TextureArgument texture )
		{
			var device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			var constantTable = ( pixelShader.Handle as SharpDX.Direct3D9.PixelShader ).Function.ConstantTable;
			var handle = constantTable.GetConstantByName ( null, texture.Uniform );
			var samplerIndex = constantTable.GetSamplerIndex ( handle );

			device.SetSamplerState ( samplerIndex, SharpDX.Direct3D9.SamplerState.MinFilter, ChangeFilter ( texture.Filter ) );
			device.SetSamplerState ( samplerIndex, SharpDX.Direct3D9.SamplerState.MagFilter, ChangeFilter ( texture.Filter ) );

			device.SetSamplerState ( samplerIndex, SharpDX.Direct3D9.SamplerState.AddressU, ChangeAddress ( texture.Addressing ) );
			device.SetSamplerState ( samplerIndex, SharpDX.Direct3D9.SamplerState.AddressV, ChangeAddress ( texture.Addressing ) );

			device.SetSamplerState ( samplerIndex, SharpDX.Direct3D9.SamplerState.MaxAnisotropy, texture.AnisotropicLevel );

			device.SetTexture ( samplerIndex, texture.Texture.Handle as SharpDX.Direct3D9.Texture );
		}

		private int ChangeFilter ( TextureFilter textureFilter )
		{
			switch ( textureFilter )
			{
				case TextureFilter.Nearest: return ( int ) SharpDX.Direct3D9.TextureFilter.Point;
				case TextureFilter.Linear: return ( int ) SharpDX.Direct3D9.TextureFilter.Linear;
				case TextureFilter.Anisotropic: return ( int ) SharpDX.Direct3D9.TextureFilter.Anisotropic;
				default: throw new ArgumentException ();
			}
		}

		private int ChangeAddress ( TextureAddressing textureAddressing )
		{
			switch ( textureAddressing )
			{
				case TextureAddressing.Wrap: return ( int ) SharpDX.Direct3D9.TextureAddress.Wrap;
				case TextureAddressing.Mirror: return ( int ) SharpDX.Direct3D9.TextureAddress.Mirror;
				case TextureAddressing.Clamp: return ( int ) SharpDX.Direct3D9.TextureAddress.Clamp;
				default: throw new ArgumentException ();
			}
		}

		public void SetTextures ( params TextureArgument [] textures )
		{
			foreach ( TextureArgument texture in textures )
				SetTexture ( texture );
		}
	}
}
