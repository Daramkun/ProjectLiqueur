using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Datas.Json;
using Daramkun.Liqueur.Nodes;
using Daramkun.Walnut.Nodes;
using Daramkun.Walnut.Scenes;

namespace Daramkun.Walnut
{
	internal static class Utilities
	{
		public static void AnalyzeJsonData ( Node node, JsonEntry entry )
		{
			if ( !( node is WalnutNode ) && !( node is WalnutScene ) ) throw new Exception ();


		}
	}
}
