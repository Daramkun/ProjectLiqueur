using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Data.Json
{
	public interface IJsonArray : IEnumerable
	{
		JsonArray ToJsonArray ();
		object FromJsonArray ( JsonArray array );
	}
}
