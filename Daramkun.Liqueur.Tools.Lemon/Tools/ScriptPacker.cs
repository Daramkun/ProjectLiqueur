using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Scripts;

namespace Daramkun.Liqueur.Tools.Lemon.Tools
{
	class ScriptPacker : ITool
	{
		public void Run ( params string [] args )
		{
			if ( args.Length < 4 || args.Length > 2 )
			{
				Console.WriteLine ( "PACKING USECASE:" );
				Console.WriteLine ( "  lemon.exe -scr <Output> <Script Language Type> <Directory>" );
				Console.WriteLine ( "APPEND USECASE:" );
				Console.WriteLine ( "  lemon.exe -scr <Destination> <Script filename>" );
				Console.WriteLine ( "FILEMOVE USECASE:" );
				Console.WriteLine ( "  lemon.exe -scr <Destination> <Packed filename> <UpDown State> <Count>" );
				Console.WriteLine ( "    Up Down State - up: Up, dn: Down" );
			}
			else if ( args.Length == 3 )
			{
				using ( Stream stream = new FileStream ( args [ 0 ], FileMode.Create ) )
				{
					BinaryWriter writer = new BinaryWriter ( stream );
					writer.Write ( Encoding.UTF8.GetBytes ( "WNSC" ) );
					writer.Write ( ( uint ) ( ScriptType ) Enum.Parse ( typeof ( ScriptType ), args [ 1 ] ) );

					string [] filenames = Directory.GetFiles ( args [ 2 ], "*", SearchOption.AllDirectories );
					string dirFullname = Path.GetDirectoryName ( filenames [ 0 ] );
					foreach ( string filename in filenames )
					{
						writer.Write ( Encoding.UTF8.GetBytes ( "SCRT" ) );
						using ( DeflateStream ds = new DeflateStream ( stream, CompressionMode.Compress, false ) )
						{
							BinaryWriter innerWriter = new BinaryWriter ( ds );
							innerWriter.Write ( Encoding.UTF8.GetBytes ( filename.Remove ( 0, dirFullname.Length + 1 ).PadLeft ( 32 ) ) );
							innerWriter.Write ( File.ReadAllText ( filename ) );
							Console.WriteLine ( "Processed: {0}", filename );
						}
					}
					writer.Write ( Encoding.UTF8.GetBytes ( "NONE" ) );
				}
			}
			else if ( args.Length == 2 )
			{
				using ( Stream stream = new FileStream ( args [ 0 ], FileMode.Open ) )
				{
					BinaryWriter writer = new BinaryWriter ( stream );
					stream.Position = stream.Length - 4;
					writer.Write ( Encoding.UTF8.GetBytes ( "SCRT" ) );
					using ( DeflateStream ds = new DeflateStream ( stream, CompressionMode.Compress, false ) )
					{
						string filename = args [ 1 ];
						BinaryWriter innerWriter = new BinaryWriter ( ds );
						innerWriter.Write ( Encoding.UTF8.GetBytes ( filename.PadLeft ( 32 ) ) );
						innerWriter.Write ( File.ReadAllText ( filename ) );
						Console.WriteLine ( "Processed: {0}", filename );
					}
					writer.Write ( Encoding.UTF8.GetBytes ( "NONE" ) );
				}
			}
			else if ( args.Length == 4 )
			{
				using ( Stream stream = new FileStream ( args [ 0 ], FileMode.Open ) )
				{
					ScriptTable scriptTable = new ScriptTable ( stream );
					stream.Position = 8;

					BinaryWriter writer = new BinaryWriter ( stream );
					foreach ( KeyValuePair<string, string> pair in scriptTable )
					{
						writer.Write ( Encoding.UTF8.GetBytes ( "SCRT" ) );
						using ( DeflateStream ds = new DeflateStream ( stream, CompressionMode.Compress, false ) )
						{
							BinaryWriter innerWriter = new BinaryWriter ( ds );
							innerWriter.Write ( Encoding.UTF8.GetBytes ( pair.Key.PadLeft ( 32 ) ) );
							innerWriter.Write ( pair.Value );
							Console.WriteLine ( "Processed: {0}", pair.Key );
						}
					}
				}
			}
		}
	}
}
