using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents.FileSystems
{
	public class LocalFileSystem : IFileSystem
	{
		string path;

		public LocalFileSystem ()
		{
			path = "";
		}

		public LocalFileSystem ( string path )
		{
			this.path = path;
		}

		public bool IsFileExist ( string filename )
		{
			return File.Exists ( Path.Combine ( path, filename ) );
		}

		public void CreateFile ( string filename )
		{
			File.Create ( Path.Combine ( path, filename ) ).Dispose ();
		}

		public void DeleteFile ( string filename )
		{
			File.Delete ( Path.Combine ( path, filename ) );
		}

		public Stream OpenFile ( string filename )
		{
			return File.Open ( Path.Combine ( path, filename ), FileMode.OpenOrCreate );
		}

		public bool IsDirectoryExist ( string directoryname )
		{
			return Directory.Exists ( directoryname );
		}

		public void CreateDirectory ( string directoryname )
		{
			Directory.CreateDirectory ( directoryname );
		}

		public void DeleteDirectory ( string directoryname )
		{
			Directory.Delete ( directoryname );
		}

		public string [] Files
		{
			get { return Directory.GetFiles ( path ); }
		}

		public string [] Directories
		{
			get { return Directory.GetDirectories ( path ); }
		}
	}
}
