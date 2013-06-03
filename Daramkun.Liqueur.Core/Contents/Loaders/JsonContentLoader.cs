using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Datas.Json;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class JsonContentLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( JsonEntry ); } }

		public bool IsAutoStreamDispose { get { return false; } }

		public object Load ( Stream stream, params object [] args )
		{
			return JsonParser.Parse ( stream );
		}
	}
}
