using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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

		public Stream OpenFile ( string filename )
		{
			return assembly.GetManifestResourceStream ( filename );
		}

		public string [] Files
		{
			get { return assembly.GetManifestResourceNames (); }
		}
	}
}
