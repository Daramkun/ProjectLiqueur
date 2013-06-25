using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Datas.Json;

namespace Daramkun.Liqueur.Exceptions
{
	public class CommonException : Exception
	{
		public CommonException ()
			: base ()
		{

		}

		public CommonException ( string message )
			: base ( message )
		{

		}

		public JsonEntry ToJson ()
		{
			JsonEntry entry = new JsonEntry ();
			entry.Add ( new JsonItem ( "exception", ToString () ) );
			entry.Add ( new JsonItem ( "hresult", HResult ) );
			entry.Add ( new JsonItem ( "message", Message ) );
			entry.Add ( new JsonItem ( "stack", StackTrace ) );
			return entry;
		}
	}
}
