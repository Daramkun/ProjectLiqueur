using System;
using System.IO;

namespace Daramkun.Liqueur.Contents
{
	public interface IFileSystem
	{
		bool IsFileExist ( string filename );
		void CreateFile ( string filename );
		void DeleteFile ( string filename );
		Stream OpenFile ( string filename );

		bool IsDirectoryExist ( string directoryname );
		void CreateDirectory ( string directoryname );
		void DeleteDirectory ( string directoryname );

		string [] Files { get; }
		string [] Directories { get; }
	}
}
