using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.FileSystems;

namespace Daramkun.Liqueur.Contents.Loaders
{
	/// <summary>
	/// ZIP Content Loader
	/// </summary>
	public class ZipContentLoader : IContentLoader
	{
		/// <summary>
		/// Content Type (ZipFileSystem)
		/// </summary>
		public Type ContentType { get { return typeof ( ZipFileSystem ); } }

		/// <summary>
		/// File Extensions
		/// </summary>
		public IEnumerable<string> FileExtensions { get { yield return "ZIP"; } }

		/// <summary>
		/// Is loaded content will be auto disposing?
		/// </summary>
		public bool IsSelfStreamDispose { get { return false; } }

		/// <summary>
		/// Load ZIP Content
		/// </summary>
		/// <param name="stream">ZIP stream</param>
		/// <param name="args">This parameter must be empty</param>
		/// <returns>Loaded ZIP</returns>
		public object Load ( Stream stream, params object [] args )
		{
			return new ZipFileSystem ( stream );
		}
	}
}
