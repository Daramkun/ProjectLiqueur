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

		private Stream GetBaseShaderStream ( ShaderType shaderType )
		{
			string shaderTypeString = null;
			switch ( shaderType )
			{
				case ShaderType.VertexShader: shaderTypeString = "VertexShader"; break;
				case ShaderType.PixelShader: shaderTypeString = "PixelShader"; break;
			}
			return Assembly.GetExecutingAssembly ().GetManifestResourceStream (
				string.Format ( "Daramkun.Liqueur.Spirit.Resources.BaseSpriteEffect.{0}.{1}.shader",
				LiqueurSystem.GraphicsDevice.BaseRenderer.ToString (), shaderTypeString ) );
		}

		public SpriteEffect ()
		{
			IShader vertexShader = LiqueurSystem.GraphicsDevice.CreateShader ( GetBaseShaderStream ( ShaderType.VertexShader ),
				ShaderType.VertexShader );
			IShader pixelShader = LiqueurSystem.GraphicsDevice.CreateShader ( GetBaseShaderStream ( ShaderType.PixelShader ),
				ShaderType.PixelShader );
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
			baseEffect.Dispose ();
		}
	}
}
