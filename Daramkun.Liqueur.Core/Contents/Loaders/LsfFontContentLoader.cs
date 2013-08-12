using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Graphics.Fonts;

namespace Daramkun.Liqueur.Contents.Loaders
{
	/// <summary>
	/// Liqueur Sprite Font Content Loader class
	/// </summary>
	public class LsfFontContentLoader : IContentLoader
	{
		/// <summary>
		/// Content Type (LsfFont)
		/// </summary>
		public Type ContentType { get { return typeof ( LsfFont ); } }

		/// <summary>
		/// File Extensions
		/// </summary>
		public IEnumerable<string> FileExtensions { get { yield return "LSF"; } }

		/// <summary>
		/// Is loaded content will be auto disposing?
		/// </summary>
		public bool IsSelfStreamDispose { get { return false; } }

		/// <summary>
		/// Load Lsf Font Content
		/// </summary>
		/// <param name="stream">Lsf Font stream</param>
		/// <param name="args">This parameter must be empty</param>
		/// <returns>Loaded Lsf Font</returns>
		public object Load ( Stream stream, params object [] args )
		{
			return new LsfFont ( stream );
		}
	}
}
