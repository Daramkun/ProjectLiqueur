using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.Mathematics;
using OpenTK.Graphics.OpenGL;

namespace Daramkun.Liqueur.Graphics
{
	class Effect : IEffect
	{
		internal int programId;
		IShader [] shaders;

		public Effect ( IGraphicsDevice graphicsDevice, params IShader [] shaders )
		{
			int effectState;

			this.shaders = shaders;

			programId = GL.CreateProgram ();
			foreach ( IShader shader in shaders )
			{
				shader.Attach ( this );
				GL.GetProgram ( programId, ProgramParameter.AttachedShaders, out effectState );
				if ( effectState == 0 )
					throw new EffectConfigurationException ();
			}
			
			GL.LinkProgram ( programId );
			GL.GetProgram ( programId, ProgramParameter.LinkStatus, out effectState );
			if ( effectState == 0 )
				throw new EffectConfigurationException ();
		}

		~Effect ()
		{
			Dispose ( false );
		}

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				foreach ( IShader shader in shaders )
				{
					shader.Detach ( this );
					shader.Dispose ();
				}
				GL.DeleteProgram ( programId );
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		public void Dispatch ( Action<IEffect> dispatchEvent )
		{
			GL.UseProgram ( programId );
			dispatchEvent ( this );
			GL.UseProgram ( 0 );
		}

		public void SetArgument<T> ( string parameter, T argument )
		{
			int uniform = GL.GetUniformLocation ( programId, parameter );
			Type baseType = argument.GetType ();
			if ( baseType == typeof ( int ) )
			{
				GL.Uniform1 ( uniform, ( int ) ( object ) argument );
			}
			else if ( baseType == typeof ( float ) )
			{
				GL.Uniform1 ( uniform, ( float ) ( object ) argument );
			}
			if ( baseType == typeof ( Vector2 ) )
			{
				Vector2 v = ( Vector2 ) ( object ) argument;
				GL.Uniform2 ( uniform, v.X, v.Y );
			}
			else if ( baseType == typeof ( Vector3 ) )
			{
				Vector3 v = ( Vector3 ) ( object ) argument;
				GL.Uniform3 ( uniform, v.X, v.Y, v.Z );
			}
			else if ( baseType == typeof ( Matrix4x4 ) )
			{
				Matrix4x4 v = ( Matrix4x4 ) ( object ) argument;
				GL.UniformMatrix4 ( uniform, 0, false, v.ToArray () );
			}
		}

		public void SetTextures ( params TextureArgument [] textures )
		{
			for ( int i = 0; i < textures.Length; i++ )
			{
				GL.ActiveTexture ( TextureUnit.Texture0 + i );
				GL.BindTexture ( TextureTarget.Texture2D, ( textures [ i ].Texture as Texture2D ).texture );

				GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, ( int ) TextureMinFilter.Linear );
				GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, ( int ) TextureMagFilter.Linear );

				GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, ( int ) TextureWrapMode.Repeat );
				GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, ( int ) TextureWrapMode.Repeat );

				int uniform = GL.GetUniformLocation ( programId, textures [ i ].Uniform );
				GL.Uniform1 ( uniform, i );
			}
		}
	}
}
