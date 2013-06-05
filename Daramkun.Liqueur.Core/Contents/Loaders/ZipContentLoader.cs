using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.FileSystems;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class ZipContentLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( ZipFileSystem ); } }
		public bool IsSelfStreamDispose { get { return true; } }

		public object Load ( Stream stream, params object [] args )
		{
			return new ZipFileSystem ( stream );
		}
	}
}
