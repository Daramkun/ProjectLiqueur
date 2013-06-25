using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daramkun.Walnut.Scripts
{
	public class NoneScriptEngine : IScriptEngine
	{
		public string ScriptLanguage { get { return ""; } }

		[Obsolete ( "Not support in This script engine.", true )]
		public void AddReference ( Assembly assembly )
		{

		}

		[Obsolete ( "Not support in This script engine.", true )]
		public void AddGlobalVariable ( string name, object var )
		{

		}

		[Obsolete ( "Not support in This script engine.", true )]
		public object Run ( string script )
		{
			return null;
		}

		[Obsolete ( "Not support in This script engine.", true )]
		public T Run<T> ( string script )
		{
			return default ( T );
		}
	}
}
