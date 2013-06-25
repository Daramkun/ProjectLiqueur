using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daramkun.Liqueur.Datas.Json;

namespace Daramkun.Walnut.Tools.PackagePacker
{
	class Program
	{
		static void Main ( string [] args )
		{
			if ( (args.Length == 0 || args.Length != 5) && args[0] != "-gen"  )
			{
				Console.WriteLine ( "USECASE:" );
				Console.WriteLine ( " FOR PACKING:" );
				Console.WriteLine ( "  wppack.exe filename info.json stringtable.stt restable.rst scripttable.rst" );
				Console.WriteLine ( " FOR INFOGEN:" );
				Console.WriteLine ( "  wppack.exe -gen" );
			}
			else if ( args [ 0 ] == "-gen" )
			{
				JsonEntry entry = new JsonEntry ();
				entry.Add ( new JsonItem ( "packageversion", "1.0" ) );
				
				Console.Write ( "PROJECT GUID>" );
				entry.Add ( new JsonItem ( "target", Console.ReadLine () ) );

				Console.Write ( "VERSION>" );
				entry.Add ( new JsonItem ( "version", Console.ReadLine () ) );

				Console.Write ( "COPYRIGHT>" );
				entry.Add ( new JsonItem ( "copyright", Console.ReadLine () ) );

				Console.Write ( "RELEASE DATE>" );
				entry.Add ( new JsonItem ( "released", Console.ReadLine () ) );

				Console.Write ( "MAIN SCENE>" );
				entry.Add ( new JsonItem ( "mainscene", Console.ReadLine () ) );

				Console.WriteLine ( entry );

				using ( FileStream stream = new FileStream ( "info.json", FileMode.Create ) )
				{
					StreamWriter sw = new StreamWriter ( stream );
					sw.Write ( entry.ToString () );
					sw.Flush ();
				}
			}
			else
			{
				using ( FileStream stream = new FileStream ( args [ 0 ], FileMode.Create ) )
				{
					using ( ZipArchive archive = new ZipArchive ( stream ) )
					{
						ZipArchiveEntry infoEntry = archive.CreateEntry ( "info.json" );
						using ( Stream infoStream = infoEntry.Open () )
						{
							byte [] infoData = File.ReadAllBytes ( args [ 1 ] );
							infoStream.Write ( infoData, 0, infoData.Length );
						}

						ZipArchiveEntry strEntry = archive.CreateEntry ( "strings.stt" );
						using ( Stream strStream = infoEntry.Open () )
						{
							byte [] strData = File.ReadAllBytes ( args [ 2 ] );
							strStream.Write ( strData, 0, strData.Length );
						}

						ZipArchiveEntry resEntry = archive.CreateEntry ( "resources.rst" );
						using ( Stream resStream = infoEntry.Open () )
						{
							byte [] resData = File.ReadAllBytes ( args [ 3 ] );
							resStream.Write ( resData, 0, resData.Length );
						}

						ZipArchiveEntry scrEntry = archive.CreateEntry ( "scripts.rst" );
						using ( Stream scrStream = infoEntry.Open () )
						{
							byte [] scrData = File.ReadAllBytes ( args [ 4 ] );
							scrStream.Write ( scrData, 0, scrData.Length );
						}
					}
				}
			}
		}
	}
}
