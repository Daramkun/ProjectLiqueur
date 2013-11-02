using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramkun.Liqueur.Tools.Lemon.Tools
{
	class ResourceTableEditor : ITool
	{
		public void Run ( params string [] args )
		{
			if ( args.Length == 0 || args [ 0 ] == "-h" )
			{
				Console.WriteLine ( "USECASE:" );
				Console.WriteLine ( "  remon.exe -res filename" );
			}
			else
			{
				CultureInfo cultureInfo = CultureInfo.CurrentCulture;
				using ( FileStream stream = new FileStream ( args [ 0 ], FileMode.OpenOrCreate, FileAccess.ReadWrite ) )
				{
					ZipArchive archive = new ZipArchive ( stream, ZipArchiveMode.Update, false, Encoding.UTF8 );

					while ( true )
					{
						Console.WriteLine ( "==========================================" );
						Console.WriteLine ( "CURRENT CULTURE: {0}", cultureInfo );
						Console.WriteLine ( "MENU:" );
						Console.WriteLine ( "  1. Add resource" );
						Console.WriteLine ( "  2. Remove resource" );
						Console.WriteLine ( "  3. Show resources of current culture" );
						Console.WriteLine ( "  4. Change current culture" );
						Console.WriteLine ( "  else. save and quit" );
						Console.Write ( ">" );

						int menuSelect;
						if ( !int.TryParse ( Console.ReadLine (), out menuSelect ) )
							menuSelect = 6;

						Console.WriteLine ( "-----------------------------------------" );

						switch ( menuSelect )
						{
							case 1:
								{
									string dir = Environment.CurrentDirectory;
									while ( true )
									{
										Console.Write ( "CMD>" );
										string cmd = Console.ReadLine ();
										if ( cmd.ToLower () == "dir" || cmd.ToLower () == "ls" )
										{
											foreach ( string d in Directory.GetDirectories ( dir ) )
												Console.WriteLine ( "[D] {0}", Path.GetFileName ( d ) );
											foreach ( string f in Directory.GetFiles ( dir ) )
												Console.WriteLine ( "[F] {0}", Path.GetFileName ( f ) );
										}
										else if ( cmd.ToLower () == "pwd" )
										{
											Console.WriteLine ( "PATH>{0}", dir );
										}
										else if ( cmd.Substring ( 0, 2 ).ToLower () == "cd" )
										{
											string folder = cmd.Substring ( 3, cmd.Length - 3 );
											if ( folder.Trim () == ".." )
											{
												List<string> temp = new List<string> ( dir.Split ( Path.DirectorySeparatorChar ) );
												temp [ 0 ] += Path.DirectorySeparatorChar;
												temp.RemoveAt ( temp.Count - 1 );
												dir = Path.Combine ( temp.ToArray () );
											}
											else
												dir = Path.Combine ( dir, folder.Trim () );
											Console.WriteLine ( "PATH>{0}", dir );
										}
										else if ( cmd.Substring ( 0, 3 ).ToLower () == "add" )
										{
											string filename = cmd.Substring ( 4, cmd.Length - 4 );
											byte [] data = File.ReadAllBytes ( Path.Combine ( dir, filename ) );
											ZipArchiveEntry entry = archive.CreateEntry ( Path.Combine ( cultureInfo.Name, filename ) );
											using ( Stream s = entry.Open () )
											{
												s.Write ( data, 0, data.Length );
											}
											break;
										}
										else if ( cmd.ToLower () == "help" )
										{
											Console.WriteLine ( "DIR\tDisplay the Files and Directories List." );
											Console.WriteLine ( "LS" );
											Console.WriteLine ( "PWD\tDisplay current directory path." );
											Console.WriteLine ( "CD\tMove to inner directory or outer directory." );
											Console.WriteLine ( "ADD\tAdd the file." );
										}
									}
								}
								break;
							case 2:
								{
									Console.Write ( "FILENAME>" );
									string cmd = Console.ReadLine ();

									ZipArchiveEntry entry = archive.GetEntry ( Path.Combine ( cultureInfo.Name, cmd ) );
									entry.Delete ();
								}
								break;
							case 3:
								{
									foreach ( ZipArchiveEntry entry in archive.Entries )
										if ( Path.GetDirectoryName ( entry.FullName ) == cultureInfo.Name )
											Console.WriteLine ( entry.Name );
								}
								break;
							case 4:
								{
									Console.Write ( "CULTURE>" );
									cultureInfo = new CultureInfo ( Console.ReadLine () );
								}
								break;
							default:
								return;
						}
					}
				}
			}
		}
	}
}
