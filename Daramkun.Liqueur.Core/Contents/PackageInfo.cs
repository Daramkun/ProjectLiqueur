using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents
{
	public struct PackageInfo
	{
		public string PackageName { get; set; }

		public string Author { get; set; }
		public string Copyright { get; set; }
		public string Description { get; set; }

		public Guid PackageID { get; set; }
		public Version Version { get; set; }
		public DateTime ReleaseDate { get; set; }

		public ImageInfo PackageCover { get; set; }

		public bool IsSubPackage { get; set; }
		public Guid [] MainPackageIDs { get; set; }

		public StringTable StringTable { get; set; }
		public ContentManager ResourceTable { get; set; }
		public ScriptTable ScriptTable { get; set; }
	}
}
