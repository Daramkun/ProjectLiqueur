using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.Math;
using OpenTK.Graphics.OpenGL;

namespace Daramkun.Liqueur.Graphics
{
	class Effect : IEffect
	{
		int programId, vertexShader, fragmentShader;

		public Effect ( Stream stream )
		{
			ZipFileSystem fileSystem = new ZipFileSystem ( stream );
			if ( fileSystem.IsFileExist ( "GLSL/main.glsl" ) )
			{
				using ( Stream programStream = fileSystem.OpenFile ( "GLSL/main.glsl" ) )
				{
					byte [] data = new byte [ programStream.Length ];
					string program = Encoding.UTF8.GetString ( data, 0, data.Length );
					programId = GL.CreateProgram ();
					GL.ShaderSource ( programId, program );
					GL.CompileShader ( programId );

					int compileState;
					GL.GetShader ( programId, ShaderParameter.CompileStatus, out compileState );
					if ( compileState == 0 )
						throw new ShaderCompileFailedException ();

					vertexShader = GL.CreateShader ( ShaderType.VertexShader );
					fragmentShader = GL.CreateShader ( ShaderType.FragmentShader );

					GL.AttachShader ( programId, vertexShader );
					GL.AttachShader ( programId, fragmentShader );
					GL.LinkProgram ( programId );
				}
			}
			else throw new ArgumentException ();
		}

		public void Dispose ()
		{
			GL.DetachShader ( programId, 0 );
			GL.DeleteProgram ( programId );
		}

		public void Dispatch ( Action<IEffect, int> dispatchEvent )
		{
			GL.UseProgram ( programId );
			dispatchEvent ( this, 0 );
			GL.UseProgram ( 0 );
		}

		public void BeginPass ( int pass )
		{

		}

		public void EndPass ()
		{

		}

		public T GetArgument<T> ( string parameter )
		{
			int uniform = GL.GetUniformLocation ( programId, parameter );
			Type baseType = typeof ( T );
			if ( baseType == typeof ( int ) )
			{
				int ret;
				GL.GetUniform ( programId, uniform, out ret );
				return ( T ) ( object ) ret;
			}
			else if ( baseType == typeof ( float ) )
			{
				float ret;
				GL.GetUniform ( programId, uniform, out ret );
				return ( T ) ( object ) ret;
			}
			if ( baseType == typeof ( Vector2 ) )
			{
				float [] ret = new float [ 2 ];
				GL.GetUniform ( programId, uniform, ret );
				return ( T ) ( object ) new Vector2 ( ret [ 0 ], ret [ 1 ] );
			}
			else if ( baseType == typeof ( Vector3 ) )
			{
				float [] ret = new float [ 3 ];
				GL.GetUniform ( programId, uniform, ret );
				return ( T ) ( object ) new Vector3 ( ret [ 0 ], ret [ 1 ], ret [ 2 ] );
			}
			else if ( baseType == typeof ( Matrix4x4 ) )
			{
				float [] ret = new float [ 4 * 4 ];
				GL.GetUniform ( programId, uniform, ret );
				return ( T ) ( object ) new Matrix4x4 (
					ret [ 0 ], ret [ 1 ], ret [ 2 ], ret [ 3 ],
					ret [ 4 ], ret [ 5 ], ret [ 6 ], ret [ 7 ],
					ret [ 8 ], ret [ 9 ], ret [ 10 ], ret [ 11 ],
					ret [ 12 ], ret [ 13 ], ret [ 14 ], ret [ 15 ]
					);
			}
			else return default ( T );
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
	}
}