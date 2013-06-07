﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Datas.Json;
using Daramkun.Liqueur.Geometries;
using Daramkun.Liqueur.Nodes;

namespace Daramkun.Walnut.Nodes
{
	public class AnchorNode : WalnutNode
	{
		public Vector2 Anchor { get; set; }

		public AnchorNode ( JsonEntry jsonEntry, ContentManager contentManager )
			: base ( jsonEntry, contentManager )
		{

		}

		public override void OnInitialize ()
		{
			Node parent = Parent;
			while ( parent != null )
			{
				if ( parent is AnchorNode )
					throw new Exception ( "" );
				parent = parent.Parent;
			}
			base.OnInitialize ();
		}
	}
}