using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Walnut.Scripts;

namespace Daramkun.Walnut.Contents
{
	public sealed class ScriptTable : IDisposable
	{
		ZipFileSystem fileSystem;

		public IScriptEngine ScriptEngine { get; set; }

		public ScriptTable ( Stream stream )
		{
			fileSystem = new ZipFileSystem ( stream );
		}

		public string GetSource ( string filename )
		{
			using ( Stream stream = fileSystem.OpenFile ( filename ) )
			{
				TextReader reader = new StreamReader ( stream, Encoding.UTF8 );
				return reader.ReadToEnd ();
			}
		}

		public object Run ( string filename )
		{
			if ( ScriptEngine == null ) return null;
			return ScriptEngine.Run ( GetSource ( filename ) );
		}

		public T Run<T> ( string filename )
		{
			if ( ScriptEngine == null ) return default ( T );
			return ScriptEngine.Run<T> ( GetSource ( filename ) );
		}

		public void Dispose ()
		{
			fileSystem.Dispose ();
		}
	}
}
