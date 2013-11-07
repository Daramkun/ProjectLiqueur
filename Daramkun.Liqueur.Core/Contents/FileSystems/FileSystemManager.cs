using System;
using System.Collections.Generic;
using Daramkun.Liqueur.Common;
using System.Reflection;

namespace Daramkun.Liqueur.Contents.FileSystems
{
	public static class FileSystemManager
	{
		static Dictionary<string, Type> fileSystems;

		static FileSystemManager ()
		{
			fileSystems = new Dictionary<string, Type> ();
			AddFileSystem ( "ManifestFileSystem", typeof ( ManifestFileSystem ) );
			AddFileSystem ( "ZipFileSystem", typeof ( ZipFileSystem ) );
		}

		public static void AddFileSystem ( string key, Type fileSystemType )
		{
			if ( !Utilities.IsSubtypeOf ( fileSystemType, typeof ( IFileSystem ) ) )
				return;
			fileSystems.Add ( key, fileSystemType );
		}

		public static IFileSystem GetFileSystem ( string key, params object [] args )
		{
			if ( key == "ManifestFileSystem" && args.Length == 0 )
				args = new object [] { Assembly.GetCallingAssembly () };
			return Activator.CreateInstance ( fileSystems [ key ], args ) as IFileSystem;
		}
	}
}

