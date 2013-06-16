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

		StorageFolder storageFolder;

		public LocalFileSystem ()
		{
			path = "";
			storageFolder = StorageFolder.GetFolderFromPathAsync ( "Documents" ).GetResults ();
		}

		public LocalFileSystem ( string path )
		{
			this.path = path;
			storageFolder = StorageFolder.GetFolderFromPathAsync ( Path.Combine ( "Documents", path ) ).GetResults ();
		}

		public bool IsFileExist ( string filename )
		{
			IReadOnlyList<StorageFile> storageFiles = storageFolder.GetFilesAsync ().GetResults ();
			foreach ( StorageFile storageFile in storageFiles )
				if ( storageFile.Name == filename ) return true;
			return false;
		}

		public void CreateFile ( string filename )
		{
			storageFolder.CreateFileAsync ( filename ).GetResults ();
		}

		public void DeleteFile ( string filename )
		{
			storageFolder.GetFileAsync ( filename ).GetResults ().DeleteAsync ().GetResults ();
		}

		public Stream OpenFile ( string filename )
		{
			return storageFolder.GetFileAsync ( filename ).GetResults ().OpenAsync ( FileAccessMode.ReadWrite ).GetResults () as Stream;
		}

		public bool IsDirectoryExist ( string directoryname )
		{
			return false;
		}

		public void CreateDirectory ( string directoryname )
		{

		}

		public void DeleteDirectory ( string directoryname )
		{

		}

		public string [] Files
		{
			get
			{
				List<string> files = new List<string> ();
				foreach ( StorageFile storageFile in storageFolder.GetFilesAsync ().GetResults () )
					files.Add ( storageFile.Name );
				return files.ToArray ();
			}
		}

		public string [] Directories
		{
			get
			{
				return new string [ 0 ];
			}
		}
	}
}
