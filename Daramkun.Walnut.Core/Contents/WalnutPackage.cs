using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Datas.Json;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Walnut.Exceptions;

namespace Daramkun.Walnut.Contents
{
	public sealed class WalnutPackage : IDisposable
	{
		public static Guid Guid { get; set; }

		ZipFileSystem fileSystem;

		public string Version { get; private set; }
		public string Copyright { get; private set; }
		public DateTime Released { get; private set; }

		public string MainScene { get; private set; }

		public StringTable StringTable { get; private set; }
		public ContentManager ResourceTable { get; private set; }
		public ScriptTable ScriptTable { get; private set; }

		public IFileSystem PackageFileSystem { get { return fileSystem; } }

		public WalnutPackage ( Stream stream )
		{
			fileSystem = new ZipFileSystem ( stream );
			JsonEntry jsonEntry = JsonParser.Parse ( fileSystem.OpenFile ( "info.json" ) );

			if ( new Version ( jsonEntry [ "packageversion" ].Data as string ).Major != 1 )
				throw new VersionMismatchException ();

			if ( new Guid ( jsonEntry [ "target" ].Data as string ) != Guid )
				throw new GameMismatchException ();

			Version = jsonEntry [ "version" ].Data as string;
			Copyright = jsonEntry [ "copyright" ].Data as string;
			Released = DateTime.Parse ( jsonEntry [ "released" ].Data as string );
			MainScene = jsonEntry [ "mainscene" ].Data as string;

			StringTable = new StringTable ( fileSystem.OpenFile ( "strings.wst" ) );
			ResourceTable = new ContentManager ( new ZipFileSystem ( fileSystem.OpenFile ( "resources.wrs" ) ) );
			ScriptTable = new ScriptTable ( fileSystem.OpenFile ( "scripts.wsc" ) );
		}

		public void Dispose ()
		{
			fileSystem.Dispose ();
		}
	}
}
