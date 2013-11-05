using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.Decoder.Packages;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class PackageContentLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( PackageInfo ); } }

		public IEnumerable<string> FileExtensions { get { yield return "WLNT"; } }

		public bool IsSelfStreamDispose
		{
			get { return true; }
		}

		public object Load ( System.IO.Stream stream, params object [] args )
		{
			return new PackageDecoder ().Decode ( stream );
		}
	}
}
