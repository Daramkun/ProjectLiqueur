using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Datas.Json
{
	public interface IJsonArray
	{
		JsonArray ToJsonArray ();
		object FromJsonArray ( JsonArray array );
	}
}
