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
	public sealed class ResourceTable : IDisposable
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

		private string PathCombine ( string path, string filename )
		{
			if ( path.IndexOf ( '\\' ) >= 0 )
			{
				if ( path [ path.Length - 1 ] == '\\' )
					return path + filename;
				else return string.Format ( "{0}\\{1}", path, filename );
			}
			else
			{
				if ( path [ path.Length - 1 ] == '/' )
					return path + filename;
				else return string.Format ( "{0}/{1}", path, filename );
			}
		}

		public object Load<T> ( string filename, params object [] args )
		{
			return contentManager.Load<T> ( PathCombine ( cultureInfo.TwoLetterISOLanguageName, filename ), args );
		}

		public void Dispose ()
		{
			contentManager.Reset ();
			contentManager.Dispose ();
		}
	}
}
