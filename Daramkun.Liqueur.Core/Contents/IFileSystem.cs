using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents
{
	/// <summary>
	/// File System interface
	/// </summary>
	public interface IFileSystem
	{
		/// <summary>
		/// Is File exist
		/// </summary>
		/// <param name="filename">Filename</param>
		/// <returns>True when file is exist, False when elsecase</returns>
		bool IsFileExist ( string filename );
		/// <summary>
		/// Open file stream
		/// </summary>
		/// <param name="filename">Filename with path</param>
		/// <returns>Opened stream</returns>
		Stream OpenFile ( string filename );
		/// <summary>
		/// Filenames with path
		/// </summary>
		string [] Files { get; }
	}

	/// <summary>
	/// Writable File System interface
	/// </summary>
	public interface IWritableFileSystem : IFileSystem
	{
		/// <summary>
		/// Create file
		/// </summary>
		/// <param name="filename">Filename with path</param>
		void CreateFile ( string filename );
		/// <summary>
		/// Delete file
		/// </summary>
		/// <param name="filename">Filename with path</param>
		void DeleteFile ( string filename );
	}

	/// <summary>
	/// Exist Directory system File System interface
	/// </summary>
	public interface IDirectorableFileSystem
	{
		/// <summary>
		/// Is Directory exist?
		/// </summary>
		/// <param name="filename">Directory name with path</param>
		/// <returns>True when directory is exist, False when elsecase</returns>
		bool IsDirectoryExist ( string directoryname );
		/// <summary>
		/// Directory names
		/// </summary>
		string [] Directories { get; }
	}

	/// <summary>
	/// Writable Exist Directory system File System interface
	/// </summary>
	public interface IWritableDirectorableFileSystem
	{
		/// <summary>
		/// Create directory
		/// </summary>
		/// <param name="directoryname">Directory name with path</param>
		void CreateDirectory ( string directoryname );
		/// <summary>
		/// Delete directory
		/// </summary>
		/// <param name="directoryname">Directory name with path</param>
		void DeleteDirectory ( string directoryname );
	}
}
