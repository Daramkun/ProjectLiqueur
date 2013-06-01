using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Graphics.Fonts;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class LsfFontContentLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( LsfFont ); } }
		public bool IsAutoStreamDispose { get { return true; } }

		public object Load ( Stream stream, params object [] args )
		{
			return new LsfFont ( stream );
		}
	}
}
