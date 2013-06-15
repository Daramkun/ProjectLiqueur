using System;
using System.Collections.Generic;
using System.IO;
using Daramkun.Liqueur.Contents.FileSystems.XUnzipSharp;

namespace Daramkun.Liqueur.Contents.FileSystems
{
	public class ZipFileSystem : IFileSystem, IDisposable
	{
		XUnzip xUnzip;
		string [] filenameCache;
		string [] dirnameCache;
		Dictionary<string, int> indexInfo;

		public ZipFileSystem ( Stream stream )
		{
			xUnzip = new XUnzip ();
			xUnzip.Open ( stream );

			indexInfo = new Dictionary<string, int> ();

			List<string> files = new List<string> ();
			List<string> dirs = new List<string> ();
			int index = 0;
			foreach ( XUnzipFileInfo info in xUnzip.FileInfo )
			{
				indexInfo.Add ( info.FileName, index++ );
				files.Add ( info.FileName );
				string [] temp = info.FileName.Split ( '/' );
				if ( temp.Length == 1 )
					temp = info.FileName.Split ( '\\' );
				temp [ temp.Length - 1 ] = "";
				string path = string.Join ( "/", temp );
				if ( dirs.Contains ( path ) )
					continue;
				dirs.Add ( path );
			}
			filenameCache = files.ToArray ();
			dirnameCache = dirs.ToArray ();
		}

		public void Dispose ()
		{
			xUnzip.Close ();
			xUnzip = null;
		}

		public bool IsFileExist ( string filename )
		{
			foreach ( XUnzipFileInfo info in xUnzip.FileInfo )
				if ( info.FileName == filename )
					return true;
			return false;
		}

		public bool IsDirectoryExist ( string directoryname )
		{
			foreach ( string dir in Directories )
				if ( dir == directoryname ) return true;
			return false;
		}

		public Stream OpenFile ( string filename )
		{
			/*for(int i = 0; i < xUnzip.FileInfo.Length; i++)
				if ( xUnzip.FileInfo [ i ].FileName == filename )
				{
					MemoryStream stream = new MemoryStream ();
					if ( !xUnzip.ExtractTo ( i, stream ) )
						return null;
					return stream;
				}*/
			/**/
			if ( !indexInfo.ContainsKey ( filename ) ) return null;

			MemoryStream stream = new MemoryStream ();
			if ( !xUnzip.ExtractTo ( indexInfo [ filename ], stream ) )
				return null;
			return stream;
			/**/
			//return null;
		}

		public string [] Files { get { return filenameCache; } }
		public string [] Directories { get { return dirnameCache; } }

		[Obsolete ( "Can't call this method in ZipFileSystem", true )]
		public void CreateFile ( string filename )
		{
			throw new NotImplementedException ();
		}

		[Obsolete ( "Can't call this method in ZipFileSystem", true )]
		public void DeleteFile ( string filename )
		{
			throw new NotImplementedException ();
		}

		[Obsolete ( "Can't call this method in ZipFileSystem", true )]
		public void CreateDirectory ( string directoryname )
		{
			throw new NotImplementedException ();
		}

		[Obsolete ( "Can't call this method in ZipFileSystem", true )]
		public void DeleteDirectory ( string directoryname )
		{
			throw new NotImplementedException ();
		}
	}
}
