using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Walnut.Scenes;

namespace Daramkun.Walnut.Contents.Loaders
{
	public class WalnutSceneContentLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( WalnutScene ); } }

		public bool IsSelfStreamDispose
		{
			get { return true; }
		}

		public object Load ( Stream stream, params object [] args )
		{
			if ( args [ 0 ] is ContentManager )
				return new WalnutScene ( stream, args [ 0 ] as ContentManager );
			else return null;
		}
	}
}
