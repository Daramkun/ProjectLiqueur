using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daramkun.Liqueur.Datas.Json;

namespace Daramkun.Liqueur.Tools.SttEdit
{
	class Program
	{
		static void Main ( string [] args )
		{
			if ( args.Length == 0 || args [ 0 ] == "-h" )
			{
				Console.WriteLine ( "USECASE:" );
				Console.WriteLine ( "  sttedit.exe filename" );
			}
			else
			{
				CultureInfo cultureInfo = CultureInfo.CurrentCulture;
				using ( FileStream stream = new FileStream ( args [ 0 ], FileMode.OpenOrCreate, FileAccess.ReadWrite ) )
				{
					JsonEntry entry;
					if ( stream.Length != 0 )
						entry = JsonParser.Parse ( stream );
					else entry = new JsonEntry ();

					while ( true )
					{
						Console.WriteLine ( "==========================================" );
						Console.WriteLine ( "CURRENT CULTURE: {0}", cultureInfo );
						Console.WriteLine ( "MENU:" );
						Console.WriteLine ( "  1. Add string" );
						Console.WriteLine ( "  2. Remove string" );
						Console.WriteLine ( "  3. Show strings of current culture" );
						Console.WriteLine ( "  4. Change current culture" );
						Console.WriteLine ( "  5. Quit without save" );
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
									Console.Write ( "KEY>" );
									string key = Console.ReadLine ();
									Console.Write ( "VALUE>" );
									string value = Console.ReadLine ();
									if ( !entry.Contains ( cultureInfo.Name ) )
										entry.Add ( new JsonItem ( cultureInfo.Name, new JsonEntry () ) );
									JsonEntry json = entry [ cultureInfo.Name ].Data as JsonEntry;
									if ( !json.Contains ( key ) )
										json.Add ( new JsonItem ( key, value ) );
									else json [ key ].Data = value;
								}
								break;
							case 2:
								{
									Console.Write ( "KEY>" );
									string key = Console.ReadLine ();
									if ( !entry.Contains ( cultureInfo.Name ) )
										break;
									JsonEntry json = entry [ cultureInfo.Name ].Data as JsonEntry;
									if ( !json.Contains ( key ) )
										break;
									json.Remove ( key );
								}
								break;
							case 3:
								{
									if ( !entry.Contains ( cultureInfo.Name ) )
										break;
									JsonEntry json = entry [ cultureInfo.Name ].Data as JsonEntry;
									foreach ( JsonItem item in json )
										Console.WriteLine ( "{0}: {1}", item.Name, item.Data );
								}
								break;
							case 4:
								{
									Console.Write ( "CULTURE>" );
									string culture = Console.ReadLine ();
									cultureInfo = new CultureInfo ( culture );
								}
								break;
							case 5:
								return;
							default:
								stream.Position = 0;
								using ( StreamWriter writer = new StreamWriter ( stream ) )
								{
									writer.Write ( entry.ToString () );
									writer.Flush ();
								}
								return;
						}
					}
				}
			}
		}
	}
}
