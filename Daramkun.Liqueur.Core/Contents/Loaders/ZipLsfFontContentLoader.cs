using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Graphics.Fonts;

namespace Daramkun.Liqueur.Contents.Loaders
{
	public class ZipLsfFontContentLoader : IContentLoader
	{
		public Type ContentType { get { return typeof ( ZipLsfFont ); } }

		public bool IsAutoStreamDispose { get { return true; } }

		public object Load ( Stream stream, params object [] args )
		{
			return new ZipLsfFont ( stream );
		}
	}
}
