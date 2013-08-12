using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daramkun.Liqueur.Contents.FileSystems
{
	/// <summary>
	/// .NET Assembly Manifest File System class
	/// </summary>
	public class ManifestFileSystem : IFileSystem
	{
		Assembly assembly;

		/// <summary>
		/// Constructor of Manifest File System class
		/// </summary>
		public ManifestFileSystem ()
		{
			assembly = Assembly.GetCallingAssembly ();
		}

		/// <summary>
		/// Constructor of Manifest File System class
		/// </summary>
		/// <param name="assembly">.NET Assembly</param>
		public ManifestFileSystem ( Assembly assembly )
		{
			this.assembly = assembly;
		}

		/// <summary>
		/// Is File exist?
		/// </summary>
		/// <param name="filename">Filename with path</param>
		/// <returns>True when file is exist, False when elsecase</returns>
		public bool IsFileExist ( string filename )
		{
			foreach ( string resname in assembly.GetManifestResourceNames () )
				if ( resname == filename ) return true;
			return false;
		}

		/// <summary>
		/// Open file stream
		/// </summary>
		/// <param name="filename">Filename with path</param>
		/// <returns>Opened stream</returns>
		public Stream OpenFile ( string filename )
		{
			return assembly.GetManifestResourceStream ( filename );
		}

		/// <summary>
		/// Filenames with path
		/// </summary>
		public string [] Files
		{
			get { return assembly.GetManifestResourceNames (); }
		}
	}
}
