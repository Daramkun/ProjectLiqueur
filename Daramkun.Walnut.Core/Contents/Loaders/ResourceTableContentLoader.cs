using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;

namespace Daramkun.Walnut.Contents.Loaders
{
	public class ResourceTableContentLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( ResourceTable ); } }

		public bool IsSelfStreamDispose { get { return true; } }

		public object Load ( Stream stream, params object [] args )
		{
			if ( args != null && args.Length == 1 )
				return new ResourceTable ( stream, args [ 0 ] as CultureInfo );
			else
				return new ResourceTable ( stream );
		}
	}
}
