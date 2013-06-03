using System;
using System.Collections.Generic;
using System.IO;
#if WINDOWS_PHONE
using System.IO.IsolatedStorage;
#endif
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents.FileSystems
{
	public class LocalFileSystem : IFileSystem
	{
		string path;

#if WINDOWS_PHONE
		IsolatedStorageFile isolatedStorageFile;
#endif

		public LocalFileSystem ()
		{
			path = "";
		}

		public LocalFileSystem ( string path )
		{
			this.path = path;
#if WINDOWS_PHONE
			isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication ();
#endif
		}

		public bool IsFileExist ( string filename )
		{
#if OPENTK
			return File.Exists ( Path.Combine ( path, filename ) );
#elif WINDOWS_PHONE
			return isolatedStorageFile.FileExists ( Path.Combine ( path, filename ) );
#endif
		}

		public void CreateFile ( string filename )
		{
#if OPENTK
			File.Create ( Path.Combine ( path, filename ) ).Dispose ();
#elif WINDOWS_PHONE
			isolatedStorageFile.CreateFile ( Path.Combine ( path, filename ) ).Dispose ();
#endif
		}

		public void DeleteFile ( string filename )
		{
#if OPENTK
			File.Delete ( Path.Combine ( path, filename ) );
#elif WINDOWS_PHONE
			isolatedStorageFile.DeleteFile ( Path.Combine ( path, filename ) );
#endif
		}

		public Stream OpenFile ( string filename )
		{
#if OPENTK
			return File.Open ( Path.Combine ( path, filename ), FileMode.OpenOrCreate );
#elif WINDOWS_PHONE
			return isolatedStorageFile.OpenFile ( Path.Combine ( path, filename ), FileMode.OpenOrCreate );
#endif
		}

		public bool IsDirectoryExist ( string directoryname )
		{
#if OPENTK
			return Directory.Exists ( directoryname );
#elif WINDOWS_PHONE
			return isolatedStorageFile.DirectoryExists ( Path.Combine ( path, directoryname ) );
#endif
		}

		public void CreateDirectory ( string directoryname )
		{
#if OPENTK
			Directory.CreateDirectory ( Path.Combine ( path, directoryname ) );
#elif WINDOWS_PHONE
			isolatedStorageFile.CreateDirectory ( Path.Combine ( path, directoryname ) );
#endif
		}

		public void DeleteDirectory ( string directoryname )
		{
#if OPENTK
			Directory.Delete ( Path.Combine ( path, directoryname ) );
#elif WINDOWS_PHONE
			isolatedStorageFile.DeleteDirectory ( Path.Combine ( path, directoryname ) );
#endif
		}

		public string [] Files
		{
			get
			{
#if OPENTK
				return Directory.GetFiles ( path );
#elif WINDOWS_PHONE
				return isolatedStorageFile.GetFileNames ( path );
#endif
			}
		}

		public string [] Directories
		{
			get
			{
#if OPENTK
				return Directory.GetDirectories ( path );
#elif WINDOWS_PHONE
				return isolatedStorageFile.GetDirectoryNames ( path );
#endif
			}
		}
	}
}
