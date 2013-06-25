using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Datas.Json;
using Daramkun.Liqueur.Nodes;
using Daramkun.Walnut.Nodes;
using Daramkun.Walnut.Scenes;

namespace Daramkun.Walnut
{
	internal static class WalnutUtilities
	{
		public static void AnalyzeJsonData ( Node node, JsonEntry entry, ContentManager contentManager )
		{
			var children = entry [ "children" ].Data as JsonArray;
			foreach ( JsonItem childItem in children )
			{
				JsonEntry child = childItem.Data as JsonEntry;

				switch ( child [ "type" ].Data as string )
				{
					case "sprite":
						node.AddChild ( new Sprite ( child, contentManager ) );
						break;
					case "label":
						node.AddChild ( new Label ( child, contentManager ) );
						break;
				}
			}
		}
	}
}
