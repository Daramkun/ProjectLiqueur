using System;
using System.Collections.Generic;
using System.IO;
#if WINDOWS_PHONE
using System.IO.IsolatedStorage;
#elif NETFX_CORE
using Windows.Storage;
#endif
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramkun.Liqueur.Contents.FileSystems
{
	public class LocalFileSystem : IFileSystem
	{
		string path;

#if WINDOWS_PHONE
		IsolatedStorageFile isolatedStorageFile;
#elif NETFX_CORE
		StorageFolder storageFolder;
#endif

		public LocalFileSystem ()
		{
			path = "";
#if WINDOWS_PHONE
			isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication ();
#elif NETFX_CORE
			storageFolder = StorageFolder.GetFolderFromPathAsync ( "" ).GetResults ();
#endif
		}

		public LocalFileSystem ( string path )
		{
			this.path = path;
#if WINDOWS_PHONE
			isolatedStorageFile = IsolatedStorageFile.GetUserStoreForApplication ();
#elif NETFX_CORE
			storageFolder = StorageFolder.GetFolderFromPathAsync ( path ).GetResults ();
#endif
		}

		public bool IsFileExist ( string filename )
		{
#if OPENTK
			return File.Exists ( Path.Combine ( path, filename ) );
#elif WINDOWS_PHONE
			return isolatedStorageFile.FileExists ( Path.Combine ( path, filename ) );
#elif NETFX_CORE
			IReadOnlyList<StorageFile> storageFiles = storageFolder.GetFilesAsync ().GetResults ();
			foreach ( StorageFile storageFile in storageFiles )
				if ( storageFile.Name == filename ) return true;
			return false;
#endif
		}

		public void CreateFile ( string filename )
		{
#if OPENTK
			File.Create ( Path.Combine ( path, filename ) ).Dispose ();
#elif WINDOWS_PHONE
			isolatedStorageFile.CreateFile ( Path.Combine ( path, filename ) ).Dispose ();
#elif NETFX_CORE
			storageFolder.CreateFileAsync ( filename ).GetResults ();
#endif
		}

		public void DeleteFile ( string filename )
		{
#if OPENTK
			File.Delete ( Path.Combine ( path, filename ) );
#elif WINDOWS_PHONE
			isolatedStorageFile.DeleteFile ( Path.Combine ( path, filename ) );
#elif NETFX_CORE
			storageFolder.GetFileAsync ( filename ).GetResults ().DeleteAsync ().GetResults ();
#endif
		}

		public Stream OpenFile ( string filename )
		{
#if OPENTK
			return File.Open ( Path.Combine ( path, filename ), FileMode.OpenOrCreate );
#elif WINDOWS_PHONE
			return isolatedStorageFile.OpenFile ( Path.Combine ( path, filename ), FileMode.OpenOrCreate );
#elif NETFX_CORE
			return storageFolder.GetFileAsync ( filename ).GetResults ().OpenAsync ( FileAccessMode.ReadWrite ).GetResults () as Stream;
#endif
		}

		public bool IsDirectoryExist ( string directoryname )
		{
#if OPENTK
			return Directory.Exists ( directoryname );
#elif WINDOWS_PHONE
			return isolatedStorageFile.DirectoryExists ( Path.Combine ( path, directoryname ) );
#elif NETFX_CORE
			return false;
#endif
		}

		public void CreateDirectory ( string directoryname )
		{
#if OPENTK
			Directory.CreateDirectory ( Path.Combine ( path, directoryname ) );
#elif WINDOWS_PHONE
			isolatedStorageFile.CreateDirectory ( Path.Combine ( path, directoryname ) );
#elif NETFX_CORE

#endif
		}

		public void DeleteDirectory ( string directoryname )
		{
#if OPENTK
			Directory.Delete ( Path.Combine ( path, directoryname ) );
#elif WINDOWS_PHONE
			isolatedStorageFile.DeleteDirectory ( Path.Combine ( path, directoryname ) );
#elif NETFX_CORE

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
#elif NETFX_CORE
				List<string> files = new List<string> ();
				foreach ( StorageFile storageFile in storageFolder.GetFilesAsync ().GetResults () )
					files.Add ( storageFile.Name );
				return files.ToArray ();
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
#elif NETFX_CORE
				return new string [ 0 ];
#endif
			}
		}
	}
}
