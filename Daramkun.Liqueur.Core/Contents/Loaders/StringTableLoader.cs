using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class StringTableLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( StringTable ); } }

		public IEnumerable<string> FileExtensions { get { yield return "LIQSTR"; } }

		public bool IsSelfStreamDispose { get { return true; } }

		public object Load ( Stream stream, params object [] args )
		{
			return new StringTable ( stream );
		}
	}
}
