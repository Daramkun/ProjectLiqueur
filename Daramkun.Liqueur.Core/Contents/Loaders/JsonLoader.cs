using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Data.Json;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class JsonLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( JsonEntry ); } }

		public bool IsSelfStreamDispose { get { return false; } }

		public IEnumerable<string> FileExtensions { get { yield return "JSON"; } }

		public object Load ( Stream stream, params object [] args )
		{
			return JsonParser.Parse ( stream );
		}
	}
}
