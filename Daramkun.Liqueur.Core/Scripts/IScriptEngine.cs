using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Exceptions;

namespace Daramkun.Liqueur.Scripts
{
	public enum ScriptType : uint
	{
		Unknown = 0,

		CSharp,
		Javascript,

		Ruby,
		Python,
		Scheme,

		Axum,
		Boo,
		Scala,
		Cobra,
		Nemerle,

		Lua,
		Perl,

		CustomScriptLanguage = UInt32.MaxValue,
	}

	public static class ScriptEngineExtension
	{
		public static void Run ( this IScriptEngine scriptEngine, ScriptTable table, string name )
		{
			if ( scriptEngine.ScriptType != table.ScriptType ) throw new ScriptLanguageMismatchException ();
			scriptEngine.Run ( table [ name ] );
		}

		public static void RunAll ( this IScriptEngine scriptEngine, ScriptTable table )
		{
			if ( scriptEngine.ScriptType != table.ScriptType ) throw new ScriptLanguageMismatchException ();
			foreach ( KeyValuePair<string, string> pair in table )
				scriptEngine.Run ( pair.Value );
		}
	}

	public interface IScriptEngine : IDisposable
	{
		ScriptType ScriptType { get; }
		Version ScriptVersion { get; }

		object Handle { get; }
		bool IsSupportDLR { get; }

		void AddAssembly ( Assembly assembly );
		object this [ string varname ] { get; set; }

		object Run ( Stream stream );
		object Run ( string script );
		T Run<T> ( Stream stream );
		T Run<T> ( string script );
	}
}
