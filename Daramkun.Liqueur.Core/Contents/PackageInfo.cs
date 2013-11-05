using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents.Decoder.Images;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Data.Json;
using Daramkun.Liqueur.Exceptions;

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

		public static PackageInfo LoadFromFileSystem ( IFileSystem fileSystem )
		{
			if ( !fileSystem.IsFileExist ( "packageInfo.json" ) )
				throw new ArgumentException ();

			PackageInfo packageInfo = new PackageInfo ();
			JsonEntry entry = JsonParser.Parse ( fileSystem.OpenFile ( "packageInfo.json" ) );
			
			packageInfo.PackageName = entry [ "title" ] as string;
			
			packageInfo.Author = entry [ "author" ] as string;
			packageInfo.Copyright = entry [ "copyright" ] as string;
			packageInfo.Description = entry [ "description" ] as string;

			packageInfo.PackageID = new Guid ( entry [ "packId" ] as string );
			packageInfo.Version = new Version ( entry [ "version" ] as string );
			packageInfo.ReleaseDate = DateTime.Parse ( entry [ "release_date" ] as string );

			if ( packageInfo.IsSubPackage = ( bool ) entry [ "issubpack" ] )
			{
				List<Guid> mainGuid = new List<Guid> ();
				JsonArray mainPackIds = entry [ "mainpacks" ] as JsonArray;
				foreach ( object item in mainPackIds )
					mainGuid.Add ( new Guid ( item as string ) );

				if ( !mainGuid.Contains ( PackageSystem.MainPackage.PackageID ) )
				{
					bool isContains = false;
					foreach ( PackageInfo subpack in PackageSystem.SubPackages )
						if ( mainGuid.Contains ( subpack.PackageID ) )
							isContains = true;
					if ( !isContains )
						throw new SubPackageNotAllowedThisPackageException ();
				}

				packageInfo.MainPackageIDs = mainGuid.ToArray ();
			}

			if ( fileSystem.IsFileExist ( "packageCover.png" ) )
				packageInfo.PackageCover = new PngDecoder ().Decode ( fileSystem.OpenFile ( "packageCover.png" ) );

			if ( fileSystem.IsFileExist ( "stringTable.stt" ) )
				packageInfo.StringTable = new StringTable ( fileSystem.OpenFile ( "stringTable.stt" ) );

			if ( fileSystem.IsFileExist ( "resourceTable.rst" ) )
				packageInfo.ResourceTable = new ContentManager ( new ZipFileSystem ( fileSystem.OpenFile ( "resourceTable.rst" ) ) );

			if ( fileSystem.IsFileExist ( "scriptTable.scr" ) )
				packageInfo.ScriptTable = new ScriptTable ( fileSystem.OpenFile ( "scriptTable.scr" ) );

			return packageInfo;
		}
	}
}
