﻿using System;
using System.Collections.Generic;
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
			var device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			var handle = ( vertexShader as Shader ).constantTable.GetConstantByName ( null, parameter );
			throw new NotImplementedException ();
		}

		public void SetArgument<T> ( string parameter, T argument )
		{
			var device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			var constantTable = ( vertexShader as Shader ).constantTable;
			var handle = constantTable.GetConstantByName ( null, parameter );

			Type baseType = argument.GetType ();
			if ( baseType == typeof ( int ) )
			{
				constantTable.SetValue<int> ( device, handle, ( int ) ( object ) argument );
			}
			else if ( baseType == typeof ( float ) )
			{
				constantTable.SetValue<float> ( device, handle, ( float ) ( object ) argument );
			}
			if ( baseType == typeof ( Vector2 ) )
			{
				Vector2 v = ( Vector2 ) ( object ) argument;
				constantTable.SetValue<Vector2> ( device, handle, ( Vector2 ) ( object ) argument );
			}
			else if ( baseType == typeof ( Vector3 ) )
			{
				Vector3 v = ( Vector3 ) ( object ) argument;
				constantTable.SetValue<Vector3> ( device, handle, ( Vector3 ) ( object ) argument );
			}
			else if ( baseType == typeof ( Vector4 ) )
			{
				Vector4 v = ( Vector4 ) ( object ) argument;
				constantTable.SetValue<Vector4> ( device, handle, ( Vector4 ) ( object ) argument );
			}
			else if ( baseType == typeof ( Matrix4x4 ) )
			{
				Matrix4x4 v = ( Matrix4x4 ) ( object ) argument;
				constantTable.SetValue ( device, handle, new SharpDX.Matrix ( ( ( Matrix4x4 ) ( object ) argument ).ToArray () ) );
			}
		}

		public void SetTexture ( TextureArgument texture )
		{
			var device = graphicsDevice.Handle as SharpDX.Direct3D9.Device;
			var constantTable = ( vertexShader as Shader ).constantTable;
			var handle = constantTable.GetConstantByName ( null, texture.Uniform );
			var samplerIndex = constantTable.GetSamplerIndex ( handle );

			device.SetTextureStageState ( samplerIndex, SharpDX.Direct3D9.TextureStage.TexCoordIndex, SharpDX.Direct3D9.TextureArgument.Texture );
			device.SetTexture ( samplerIndex, texture.Texture.Handle as SharpDX.Direct3D9.Texture );
		}

		public void SetTextures ( params TextureArgument [] textures )
		{
			foreach ( TextureArgument texture in textures )
				SetTexture ( texture );
		}
	}
}