using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Data.Json;

namespace Daramkun.Liqueur.Contents.Loaders
{
	/// <summary>
	/// Json Content Loader class
	/// </summary>
	public class JsonContentLoader : IContentLoader
	{
		/// <summary>
		/// Content Type (JsonEntry)
		/// </summary>
		public Type ContentType { get { return typeof ( JsonEntry ); } }

		/// <summary>
		/// Is loaded content will be auto disposing?
		/// </summary>
		public bool IsSelfStreamDispose { get { return false; } }

		/// <summary>
		/// File Extensions
		/// </summary>
		public IEnumerable<string> FileExtensions { get { yield return "JSON"; } }

		/// <summary>
		/// Load JSON Content
		/// </summary>
		/// <param name="stream">JSON stream</param>
		/// <param name="args">This parameter must be empty</param>
		/// <returns>Loaded Json Entry</returns>
		public object Load ( Stream stream, params object [] args )
		{
			return JsonParser.Parse ( stream );
		}
	}
}
