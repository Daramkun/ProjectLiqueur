using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents.Loaders
{
	/// <summary>
	/// Text Content Loader class
	/// </summary>
	public class TextContentLoader : IContentLoader
	{
		/// <summary>
		/// Content Type (string)
		/// </summary>
		public Type ContentType { get { return typeof ( string ); } }

		/// <summary>
		/// File Extensions
		/// </summary>
		public IEnumerable<string> FileExtensions { get { yield return "TXT"; } }

		/// <summary>
		/// Is loaded content will be auto disposing?
		/// </summary>
		public bool IsSelfStreamDispose { get { return true; } }

		/// <summary>
		/// Load TXT Content
		/// (If you unset args, Default Encoding is UTF8)
		/// </summary>
		/// <param name="stream">TXT stream</param>
		/// <param name="args">If you want to use Encoding class, set this argument</param>
		/// <returns>Loaded Text</returns>
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
