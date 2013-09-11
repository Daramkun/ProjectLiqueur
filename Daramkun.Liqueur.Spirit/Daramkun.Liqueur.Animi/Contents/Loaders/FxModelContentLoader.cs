using System;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Animi.Graphics;
using System.IO;
using System.Collections.Generic;

namespace Daramkun.Liqueur.Animi.Contents.Loaders
{
	public class FxModelContentLoader<T> : IContentLoader
	{
		public Type ContentType { get { return typeof ( Model<T> ); } }
		public IEnumerable<string> FileExtensions { get { yield return "FX"; } }
		public bool IsSelfStreamDispose { get { return true; } }

		public object Load ( Stream stream, params object[] args )
		{
			throw new NotImplementedException ();
		}
	}
}

