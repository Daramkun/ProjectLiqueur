using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Daramkun.Liqueur.Data.Json;

namespace Daramkun.Liqueur.Tools.Lemon.Tools
{
	class Packer : ITool
	{
		public void Run ( params string [] args )
		{
			if ( args.Length < 5 )
			{
				Console.WriteLine ( "USECASE:" );
				Console.WriteLine ( "  lemon.exe -pck <Output> <Proj Info> <Str Table> <Res Table> [<Scr Table>] [<Cover>]" );
			}
			else
			{
				if ( !File.Exists ( args [ 1 ] ) ) ErrorRaise ();
				if ( !File.Exists ( args [ 2 ] ) ) ErrorRaise ();
				if ( !File.Exists ( args [ 3 ] ) ) ErrorRaise ();
				if ( !File.Exists ( args [ 4 ] ) ) ErrorRaise ();

				using ( Stream output = new FileStream ( args [ 0 ], FileMode.Create ) )
				{
					JsonEntry jsonEntry = JsonParser.Parse ( new FileStream ( args [ 1 ], FileMode.Open ) );
					byte [] stringTable = File.ReadAllBytes ( args [ 2 ] );
					byte [] resourceTable = File.ReadAllBytes ( args [ 3 ] );
					byte [] scriptTable = ( args.Length <= 5 ) ? ( ( File.Exists ( args [ 4 ] ) ) ? File.ReadAllBytes ( args [ 4 ] ) : null ) : null;
					byte [] coverImage = ( args.Length == 6 ) ? ( ( File.Exists ( args [ 5 ] ) ) ? File.ReadAllBytes ( args [ 5 ] ) : null ) : null;

					BinaryWriter writer = new BinaryWriter ( output );
					writer.Write ( Encoding.UTF8.GetBytes ( "WLNT" ) );

					using ( DeflateStream df = new DeflateStream ( output, CompressionMode.Compress, false ) )
					{
						writer = new BinaryWriter ( df );

						writer.Write ( Encoding.UTF8.GetBytes ( ( jsonEntry [ "title" ] as string ).PadLeft ( 32, '\0' ) ) );

						writer.Write ( Encoding.UTF8.GetBytes ( ( jsonEntry [ "author" ] as string ).PadLeft ( 32, '\0' ) ) );
						writer.Write ( Encoding.UTF8.GetBytes ( ( jsonEntry [ "copyright" ] as string ).PadLeft ( 128, '\0' ) ) );
						writer.Write ( Encoding.UTF8.GetBytes ( ( jsonEntry [ "description" ] as string ).PadLeft ( 128, '\0' ) ) );

						writer.Write ( new Guid ( jsonEntry [ "packId" ] as string ).ToByteArray () );
						SaveVersion ( writer, new Version ( jsonEntry [ "version" ] as string ) );
						SaveReleaseDate ( writer, DateTime.Today );

						if ( coverImage == null ) writer.Write ( ( int ) 0 );
						else { writer.Write ( coverImage.Length ); writer.Write ( coverImage ); }

						if ( jsonEntry.Contains ( "subpacks" ) )
						{
							writer.Write ( true );

							JsonArray array = jsonEntry [ "subpacks" ] as JsonArray;
							writer.Write ( ( byte ) array.Count );
							foreach ( object arr in array )
							{
								writer.Write ( new Guid ( arr as string ).ToByteArray () );
							}
						}
						else writer.Write ( false );

						writer.Write ( stringTable.Length );
						writer.Write ( stringTable );
						writer.Write ( resourceTable.Length );
						writer.Write ( resourceTable );
						if ( scriptTable != null )
						{
							writer.Write ( scriptTable.Length );
							writer.Write ( scriptTable );
						}
						else writer.Write ( ( int ) 0 );
					}
				}
			}
		}

		private void SaveVersion ( BinaryWriter writer, Version version )
		{
			writer.Write ( ( byte ) version.Major );
			writer.Write ( ( byte ) version.Minor );
			writer.Write ( ( byte ) version.Build );
			writer.Write ( ( ushort ) version.Revision );
		}

		private void SaveReleaseDate ( BinaryWriter writer, DateTime dateTime )
		{
			writer.Write ( ( short ) dateTime.Year );
			writer.Write ( ( byte ) dateTime.Month );
			writer.Write ( ( byte ) dateTime.Day );
		}

		private void ErrorRaise ()
		{
			Console.WriteLine ( "ERROR!" );
			Thread.CurrentThread.Abort ();
		}
	}
}
