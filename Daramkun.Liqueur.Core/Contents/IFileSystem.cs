using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents
{
	public interface IFileSystem
	{
		bool IsFileExist ( string filename );

		Stream OpenFile ( string filename );

		string [] Files { get; }
	}

	public interface IWritableFileSystem : IFileSystem
	{
		void CreateFile ( string filename );
		void DeleteFile ( string filename );
	}

	public interface IDirectorableFileSystem
	{
		bool IsDirectoryExist ( string directoryname );

		string [] Directories { get; }
	}

	public interface IWritableDirectorableFileSystem
	{
		void CreateDirectory ( string directoryname );
		void DeleteDirectory ( string directoryname );
	}
}
