using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using IronRuby;
using Microsoft.Scripting.Hosting;

namespace Daramkun.Walnut.Scripts
{
	public class RubyScriptEngine : IScriptEngine
	{
		ScriptEngine scriptEngine;

		public string ScriptLanguage { get { return "Ruby"; } }

		public RubyScriptEngine ()
		{
			scriptEngine = Ruby.CreateEngine ();
		}

		public void AddReference ( Assembly assembly )
		{
			scriptEngine.Runtime.LoadAssembly ( assembly );
		}

		public void AddGlobalVariable ( string name, object var )
		{
			scriptEngine.Runtime.Globals.SetVariable ( name, var );
		}

		public object Run ( string script )
		{
			return scriptEngine.Execute ( script );
		}

		public T Run<T> ( string script )
		{
			return scriptEngine.Execute<T> ( script );
		}
	}
}
