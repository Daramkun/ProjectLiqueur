using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Spirit.Graphics;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class TrueTypeFontContentLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( TrueTypeFont ); } }

		public IEnumerable<string> FileExtensions { get { yield return "LSF"; } }

		public bool IsSelfStreamDispose { get { return false; } }

		public object Load ( Stream stream, params object [] args )
		{
			return new TrueTypeFont ( stream, ( int ) args [ 0 ] );
		}
	}
}
