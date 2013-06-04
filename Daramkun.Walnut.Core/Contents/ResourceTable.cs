using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.FileSystems;

namespace Daramkun.Walnut.Contents
{
	public class ResourceTable
	{
		ContentManager contentManager;
		CultureInfo cultureInfo;

		public void AddContentLoader ( IContentLoader contentLoader )
		{
			contentManager.AddContentLoader ( contentLoader );
		}

		public ResourceTable ( Stream stream )
		{
			SetupResourceTable ( stream, CultureInfo.CurrentCulture );
		}

		public ResourceTable ( Stream stream, CultureInfo cultureInfo )
		{
			SetupResourceTable ( stream, cultureInfo );
		}

		private void SetupResourceTable ( Stream stream, CultureInfo cultureInfo )
		{
			contentManager = new ContentManager () { FileSystem = new ZipFileSystem ( stream ) };
			contentManager.AddDefaultContentLoader ();

			this.cultureInfo = cultureInfo;
		}
	}
}
