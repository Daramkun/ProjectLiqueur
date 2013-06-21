using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.FileSystems;
using OpenTK.Graphics.OpenGL;

namespace Daramkun.Liqueur.Graphics
{
	class Effect : IEffect
	{
		int programId;

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
				}
			}
			else throw new ArgumentException ();
		}

		public void Dispose ()
		{
			GL.DeleteProgram ( programId );
		}

		public void Dispatch ( Action<IEffect, int> dispatchEvent )
		{
			GL.LinkProgram ( programId );
			dispatchEvent ( this, 0 );
			GL.LinkProgram ( 0 );
		}

		public void BeginPass ( int pass )
		{

		}

		public void EndPass ()
		{

		}

		public T GetArgument<T> ( string parameter )
		{
			throw new NotImplementedException ();
		}

		public void SetArgument<T> ( string parameter, T argument )
		{

		}

		public object this [ string parameter ]
		{
			get { return GetArgument<object> ( parameter ); }
			set { SetArgument<object> ( parameter, value ); }
		}
	}
}