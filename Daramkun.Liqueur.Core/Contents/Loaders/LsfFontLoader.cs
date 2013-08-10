using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Graphics.Fonts;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class LsfFontLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( LsfFont ); } }

		public IEnumerable<string> FileExtensions { get { yield return "LSF"; } }

		public bool IsSelfStreamDispose { get { return false; } }

		public object Load ( Stream stream, params object [] args )
		{
			return new LsfFont ( stream );
		}
	}
}
