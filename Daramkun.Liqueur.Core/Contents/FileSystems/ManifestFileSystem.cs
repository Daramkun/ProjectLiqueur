using System;
using System.Diagnostics;
using System.Reflection;

namespace Daramkun.Liqueur.Contents.FileSystems
{
	public class ManifestFileSystem : IFileSystem
	{
		Assembly assembly;

		public ManifestFileSystem ()
		{
			assembly = Assembly.GetCallingAssembly ();
		}

		public ManifestFileSystem ( Assembly assembly )
		{
			this.assembly = assembly;
		}

		public bool IsFileExist ( string filename )
		{
			foreach ( string resname in assembly.GetManifestResourceNames () )
				if ( resname == filename ) return true;
			return false;
		}

		[Obsolete ( "Cannot use this Method from ManifestFileSystem", true )]
		public void CreateFile ( string filename )
		{
			throw new NotImplementedException ();
		}

		[Obsolete ( "Cannot use this Method from ManifestFileSystem", true )]
		public void DeleteFile ( string filename )
		{
			throw new NotImplementedException ();
		}

		public System.IO.Stream OpenFile ( string filename )
		{
			return assembly.GetManifestResourceStream ( filename );
		}

		[Obsolete ( "Cannot use this Method from ManifestFileSystem", true )]
		public bool IsDirectoryExist ( string directoryname )
		{
			throw new NotImplementedException ();
		}

		[Obsolete ( "Cannot use this Method from ManifestFileSystem", true )]
		public void CreateDirectory ( string directoryname )
		{
			throw new NotImplementedException ();
		}

		[Obsolete ( "Cannot use this Method from ManifestFileSystem", true )]
		public void DeleteDirectory ( string directoryname )
		{
			throw new NotImplementedException ();
		}

		public string [] Files
		{
			get { return assembly.GetManifestResourceNames (); }
		}

		[Obsolete ( "Cannot use this Property from ManifestFileSystem", true )]
		public string [] Directories
		{
			get { return new string [] { }; }
		}
	}
}
