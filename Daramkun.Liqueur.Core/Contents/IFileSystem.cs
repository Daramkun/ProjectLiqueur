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
