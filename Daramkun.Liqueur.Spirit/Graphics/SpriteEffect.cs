using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Spirit.Graphics
{
	public sealed class SpriteEffect : IEffect
	{
		private IEffect baseEffect;

		public object Handle { get { return baseEffect.Handle; } }

		private Stream GetBaseShaderStream ( ShaderType shaderType, int glslVersion = 0 )
		{
			string shaderTypeString = null;
			switch ( shaderType )
			{
				case ShaderType.VertexShader: shaderTypeString = "VertexShader"; break;
				case ShaderType.PixelShader: shaderTypeString = "PixelShader"; break;
			}

			string rendererType = LiqueurSystem.GraphicsDevice.BaseRenderer.ToString ();
			if ( LiqueurSystem.GraphicsDevice.BaseRenderer == BaseRenderer.OpenGL )
			{
				if ( LiqueurSystem.GraphicsDevice.RendererVersion.Major <= 2 || glslVersion == 10200 )
					rendererType += "2";
				else
					rendererType += "3";
			}

			return Assembly.GetExecutingAssembly ().GetManifestResourceStream (
				string.Format ( "Daramkun.Liqueur.Spirit.Resources.BaseSpriteEffect.{0}.{1}.shader",
				rendererType, shaderTypeString ) );
		}

		public SpriteEffect ()
		{
			IShader vertexShader = null;
			try
			{
				vertexShader = LiqueurSystem.GraphicsDevice.CreateShader ( GetBaseShaderStream ( ShaderType.VertexShader ),
					ShaderType.VertexShader );
			}
			catch ( Exception ex )
			{
				if ( LiqueurSystem.GraphicsDevice.BaseRenderer == BaseRenderer.OpenGL )
				{
					try
					{
						vertexShader = LiqueurSystem.GraphicsDevice.CreateShader (
							GetBaseShaderStream ( ShaderType.VertexShader, 10200 ), ShaderType.VertexShader
						);
					}
					catch
					{
						throw ex;
					}
				}
			}

			if ( LiqueurSystem.GraphicsDevice.RendererVersion.Major == 2 )
			{
				vertexShader.Option = new ShaderOption ()
				{
					AttributeOrdering = new ShaderOption.AttributeOrder []
					{
						new ShaderOption.AttributeOrder() { Name = "i_position", VertexType = FlexibleVertexFormat.PositionXY },
						new ShaderOption.AttributeOrder() { Name = "i_overlay", VertexType = FlexibleVertexFormat.Diffuse },
						new ShaderOption.AttributeOrder() { Name = "i_texture", VertexType = FlexibleVertexFormat.TextureUV1 },
					}
				};
			}

			IShader pixelShader = null;
			try
			{
				pixelShader = LiqueurSystem.GraphicsDevice.CreateShader ( GetBaseShaderStream ( ShaderType.PixelShader ),
					ShaderType.PixelShader );
			}
			catch ( Exception ex )
			{
				if ( LiqueurSystem.GraphicsDevice.BaseRenderer == BaseRenderer.OpenGL )
				{
					try
					{
						pixelShader = LiqueurSystem.GraphicsDevice.CreateShader ( 
							GetBaseShaderStream ( ShaderType.PixelShader, 10200 ), ShaderType.PixelShader
						);
					}
					catch
					{
						throw ex;
					}
				}
			}
			baseEffect = LiqueurSystem.GraphicsDevice.CreateEffect ( vertexShader, pixelShader );
		}

		public void Dispatch ( Action<IEffect> dispatchEvent )
		{
			baseEffect.Dispatch ( dispatchEvent );
		}

		public T GetArgument<T> ( string parameter ) { return baseEffect.GetArgument<T> ( parameter ); }
		public void SetArgument<T> ( string parameter, T argument ) { baseEffect.SetArgument<T> ( parameter, argument ); }

		public void SetTextures ( params TextureArgument [] textures )
		{
			baseEffect.SetTextures ( textures );
		}

		public void Dispose ()
		{
			if ( baseEffect == null ) return;
			baseEffect.Dispose ();
			baseEffect = null;
		}
	}
}
