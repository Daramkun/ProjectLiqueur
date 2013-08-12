using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents
{
	/// <summary>
	/// Content Loader interface
	/// </summary>
	public interface IContentLoader
	{
		/// <summary>
		/// Content Type
		/// </summary>
		Type ContentType { get; }
		/// <summary>
		/// File Extensions
		/// </summary>
		IEnumerable<string> FileExtensions { get; }
		/// <summary>
		/// Is loaded content will be auto disposing?
		/// </summary>
		bool IsSelfStreamDispose { get; }
		/// <summary>
		/// Load Content
		/// </summary>
		/// <param name="stream">Content stream</param>
		/// <param name="args">Argument, if you need</param>
		/// <returns>Loaded Content</returns>
		object Load ( Stream stream, params object [] args );
	}
}
