using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public sealed class StringTableContentLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( StringTable ); } }

		public bool IsSelfStreamDispose { get { return false; } }

		public object Load ( Stream stream, params object [] args )
		{
			return new StringTable ( stream );
		}
	}
}
