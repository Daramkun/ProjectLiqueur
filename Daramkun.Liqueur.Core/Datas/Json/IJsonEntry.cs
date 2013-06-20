﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Datas.Json
{
	public interface IJsonEntry
	{
		JsonEntry ToJsonEntry ();
		object FromJsonEntry ( JsonEntry entry );
	}
}
