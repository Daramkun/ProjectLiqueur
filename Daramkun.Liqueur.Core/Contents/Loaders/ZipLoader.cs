using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.FileSystems;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class ZipLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( ZipFileSystem ); } }

		public IEnumerable<string> FileExtensions { get { yield return "ZIP"; } }

		public bool IsSelfStreamDispose { get { return false; } }

		public object Load ( Stream stream, params object [] args )
		{
			return new ZipFileSystem ( stream );
		}
	}
}
