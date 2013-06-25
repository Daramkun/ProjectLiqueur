using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daramkun.Walnut.Scripts
{
	public interface IScriptEngine
	{
		string ScriptLanguage { get; }

		void AddReference ( Assembly assembly );
		void AddGlobalVariable ( string name, object var );

		object Run ( string script );
		T Run<T> ( string script );
	}
}
