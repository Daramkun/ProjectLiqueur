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

		public object Handle { get { return programId; } }

		public Effect ( IGraphicsDevice graphicsDevice, params IShader [] shaders )
		{
			int effectState;

			this.shaders = shaders;

			programId = GL.CreateProgram ();
			foreach ( IShader shader in shaders )
			{
				( shader as Shader ).Attach ( this );
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
					( shader as Shader ).Detach ( this );
					shader.Dispose ();
				}
				GL.DeleteProgram ( programId );
				programId = 0;
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		public void Dispatch ( Action<IEffect> dispatchEvent )
		{
			int lastProgram;
			GL.GetInteger ( GetPName.CurrentProgram, out lastProgram );
			GL.UseProgram ( programId );
			dispatchEvent ( this );
			GL.UseProgram ( lastProgram );
		}

		public T GetArgument<T> ( string parameter ) where T : struct
		{
			int lastProgram;
			GL.GetInteger ( GetPName.CurrentProgram, out lastProgram );
			GL.UseProgram ( programId );
			int uniform = GL.GetUniformLocation ( programId, parameter );
			Type baseType = typeof ( T );
			T returnValue = default ( T );
			if ( baseType == typeof ( int ) )
			{
				int param;
				GL.GetUniform ( programId, uniform, out param );
				returnValue = ( T ) ( object ) param;
			}
			else if ( baseType == typeof ( float ) )
			{
				float param;
				GL.GetUniform ( programId, uniform, out param );
				returnValue = ( T ) ( object ) param;
			}
			else if ( baseType == typeof ( Vector2 ) )
			{
				float [] param = new float [ 2 ];
				GL.GetUniform ( programId, uniform, param );
				return ( T ) ( object ) new Vector2 ( param [ 0 ], param [ 1 ] );
			}
			else if ( baseType == typeof ( Vector3 ) )
			{
				float [] param = new float [ 3 ];
				GL.GetUniform ( programId, uniform, param );
				returnValue = ( T ) ( object ) new Vector3 ( param [ 0 ], param [ 1 ], param [ 2 ] );
			}
			else if ( baseType == typeof ( Vector4 ) )
			{
				float [] param = new float [ 4 ];
				GL.GetUniform ( programId, uniform, param );
				returnValue = ( T ) ( object ) new Vector4 ( param [ 0 ], param [ 1 ], param [ 2 ], param [ 3 ] );
			}
			else if ( baseType == typeof ( Matrix4x4 ) )
			{
				float [] param = new float [ 16 ];
				GL.GetUniform ( programId, uniform, param );
				returnValue = ( T ) ( object ) new Matrix4x4 (
					param [ 0 ], param [ 1 ], param [ 2 ], param [ 3 ],
					param [ 4 ], param [ 5 ], param [ 6 ], param [ 7 ],
					param [ 8 ], param [ 9 ], param [ 10 ], param [ 11 ],
					param [ 12 ], param [ 13 ], param [ 14 ], param [ 15 ]
				);
			}
			GL.UseProgram ( lastProgram );
			
			return returnValue;
		}

		public void SetArgument<T> ( string parameter, T argument ) where T : struct
		{
			int lastProgram;
			GL.GetInteger ( GetPName.CurrentProgram, out lastProgram );
			GL.UseProgram ( programId );
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
			else if ( baseType == typeof ( Vector4 ) )
			{
				Vector4 v = ( Vector4 ) ( object ) argument;
				GL.Uniform4 ( uniform, v.X, v.Y, v.Z, v.W );
			}
			else if ( baseType == typeof ( Matrix4x4 ) )
			{
				Matrix4x4 v = ( Matrix4x4 ) ( object ) argument;
				GL.UniformMatrix4 ( uniform, 1, false, v.ToArray () );
			}
			GL.UseProgram ( lastProgram );
		}

		public void SetTexture ( TextureArgument texture )
		{
			int lastProgram;
			GL.GetInteger ( GetPName.CurrentProgram, out lastProgram );

			GL.UseProgram ( programId );

			GL.ActiveTexture ( TextureUnit.Texture0 );
			GL.BindTexture ( TextureTarget.Texture2D, ( texture.Texture as Texture2D ).texture );

			GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, GetFilter ( texture.Filter ) );
			GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, GetFilter ( texture.Filter ) );

			GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, GetAddressing ( texture.Addressing ) );
			GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, GetAddressing ( texture.Addressing ) );

			int uniform = GL.GetUniformLocation ( programId, texture.Uniform );
			GL.Uniform1 ( uniform, 0 );

			GL.UseProgram ( lastProgram );
		}

		public void SetTextures ( params TextureArgument [] textures )
		{
			int lastProgram;
			GL.GetInteger ( GetPName.CurrentProgram, out lastProgram );
			GL.UseProgram ( programId );
			for ( int i = 0; i < textures.Length; i++ )
			{
				GL.ActiveTexture ( TextureUnit.Texture0 + i );
				GL.BindTexture ( TextureTarget.Texture2D, ( textures [ i ].Texture as Texture2D ).texture );

				GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, GetFilter ( textures [ i ].Filter ) );
				GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, GetFilter ( textures [ i ].Filter ) );

				GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapS, GetAddressing ( textures [ i ].Addressing ) );
				GL.TexParameter ( TextureTarget.Texture2D, TextureParameterName.TextureWrapT, GetAddressing ( textures [ i ].Addressing ) );

				int uniform = GL.GetUniformLocation ( programId, textures [ i ].Uniform );
				GL.Uniform1 ( uniform, i );
			}
			GL.UseProgram ( lastProgram );
		}

		private int GetAddressing ( TextureAddressing textureAddressing )
		{
			switch ( textureAddressing )
			{
				case TextureAddressing.Mirror: return ( int ) TextureWrapMode.MirroredRepeat;
				case TextureAddressing.Clamp: return ( int ) TextureWrapMode.Clamp;
				default:
				case TextureAddressing.Wrap: return ( int ) TextureWrapMode.Repeat;
			}
		}

		private int GetFilter ( TextureFilter textureFilter )
		{
			switch ( textureFilter )
			{
				case TextureFilter.Linear: return ( int ) TextureMinFilter.Linear;

				default:
				case TextureFilter.Nearest: return ( int ) TextureMinFilter.Nearest;
			}
		}
	}
}
