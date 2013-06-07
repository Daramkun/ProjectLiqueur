using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramkun.Walnut.Tools.ResourcePacker
{
	class Program
	{
		static void Main ( string [] args )
		{
			if ( args.Length == 0 || args [ 0 ] == "-h" || args.Length < 3 )
			{
				#region Display help
				Console.WriteLine ( "USECASE:" );
				Console.WriteLine ( "  respack.exe dest -[a|d|r] -[langcode] filename" );
				Console.WriteLine ( "===============================================================================" );
				Console.WriteLine ( "          dest:      dest - wrs filename" );
				Console.WriteLine ( "      -[a|d|r]:         a - add file" );
				Console.WriteLine ( "                        d - delete file" );
				Console.WriteLine ( "                        r - replace file" );
				Console.WriteLine ( "    -[langcode]: langcode - language code (C# CultureInfo value)" );
				Console.WriteLine ( "       filename: filename - filename for add or delete" );
				#endregion
			}
			else
			{
				string destination = args [ 0 ];
				string addMode = args [ 1 ];
				string langcode = ( args.Length == 4 ) ? args [ 2 ] : CultureInfo.CurrentCulture.Name;
				string filename = ( args.Length == 4 ) ? args [ 3 ] : args [ 2 ];

				CultureInfo cultureInfo = new CultureInfo ( langcode );

				using ( FileStream fileStream = new FileStream ( destination, FileMode.OpenOrCreate ) )
				{
					using ( ZipArchive archive = new ZipArchive ( fileStream ) )
					{
						if ( addMode == "-a" || addMode == "-r" )
						{
							ZipArchiveEntry entry;
							if ( addMode == "-a" )
								entry = archive.CreateEntry ( Path.Combine ( cultureInfo.Name, Path.GetFileName ( filename ) ) );
							else
								entry = archive.GetEntry ( Path.Combine ( cultureInfo.Name, Path.GetFileName ( filename ) ) );
							using ( Stream stream = entry.Open () )
							{
								using ( FileStream sourceFile = new FileStream ( filename, FileMode.Open, FileAccess.Read ) )
								{
									byte [] data = new byte [ sourceFile.Length ];
									sourceFile.Read ( data, 0, data.Length );
									stream.Write ( data, 0, data.Length );
								}
							}
						}
						else if ( addMode == "-d" )
						{
							archive.GetEntry ( Path.Combine ( cultureInfo.Name, Path.GetFileName ( filename ) ) ).Delete ();
						}
					}
				}
			}
		}
	}
}
