using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents.Loaders
{
	/// <summary>
	/// String Table Content Loader
	/// </summary>
	public class StringTableContentLoader : IContentLoader
	{
		/// <summary>
		/// Content Type (StringTable)
		/// </summary>
		public Type ContentType { get { return typeof ( StringTable ); } }

		/// <summary>
		/// File Extensions
		/// </summary>
		public IEnumerable<string> FileExtensions { get { yield return "LIQSTR"; } }

		/// <summary>
		/// Is loaded content will be auto disposing?
		/// </summary>
		public bool IsSelfStreamDispose { get { return true; } }

		/// <summary>
		/// Loaded LIQSTR Content
		/// </summary>
		/// <param name="stream">LiqStr stream</param>
		/// <param name="args">This parameter must be empty</param>
		/// <returns>Loaded LiqStr String Table</returns>
		public object Load ( Stream stream, params object [] args )
		{
			return new StringTable ( stream );
		}
	}
}
