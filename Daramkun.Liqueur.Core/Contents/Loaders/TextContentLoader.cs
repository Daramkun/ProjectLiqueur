using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class TextContentLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( string ); } }
		public bool IsSelfStreamDispose { get { return false; } }

		public object Load ( Stream stream, params object [] args )
		{
			Encoding encoding = Encoding.UTF8;
			if ( args != null && args.Length == 1 )
				encoding = args [ 0 ] as Encoding;
			StreamReader reader = new StreamReader ( stream, encoding );
			return reader.ReadToEnd ();
		}
	}
}
