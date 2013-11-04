using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.IO.Compression;
using Daramkun.Liqueur.Scripts;

namespace Daramkun.Liqueur.Contents
{
	public class ScriptTable : IEnumerable<KeyValuePair<string, string>>
	{
		List<KeyValuePair<string, string>> scriptTable = new List<KeyValuePair<string, string>> ();
		public ScriptType ScriptType { get; private set; }

		public ScriptTable ( Stream stream )
		{
			BinaryReader reader = new BinaryReader ( stream );
			if ( Encoding.UTF8.GetString ( reader.ReadBytes ( 4 ), 0, 4 ) != "WNSC" )
				throw new FileFormatMismatchException ();
			DeflateStream ds = new DeflateStream ( stream, CompressionMode.Decompress );
			reader = new BinaryReader ( ds );
			ScriptType = ( ScriptType ) reader.ReadUInt32 ();

			while ( Encoding.UTF8.GetString ( reader.ReadBytes ( 4 ), 0, 4 ) == "SCRT" )
			{
				string name = Encoding.UTF8.GetString ( reader.ReadBytes ( 32 ), 0, 32 ).Trim ( '\0', ' ', '\t', '\n', '　' );
				string code = reader.ReadString ();
				scriptTable.Add ( new KeyValuePair<string, string> ( name, code ) );
			}
		}

		public string this [ string name ]
		{
			get
			{
				foreach ( KeyValuePair<string, string> pair in from p in scriptTable where p.Key == name select p )
					return pair.Value;
				throw new FileNotFoundException ();
			}
		}

		public bool Contains ( string name )
		{
			foreach ( KeyValuePair<string, string> pair in from p in scriptTable where p.Key == name select p )
				return true;
			return false;
		}

		public void MoveToUp ( string name )
		{
			foreach ( KeyValuePair<string, string> pair in from p in scriptTable where p.Key == name select p )
			{
				int index = scriptTable.IndexOf ( pair );
				scriptTable [ index ] = scriptTable [ index - 1 ];
				scriptTable [ index - 1 ] = pair;
				break;
			}
		}

		public void MoveToDown ( string name )
		{
			foreach ( KeyValuePair<string, string> pair in from p in scriptTable where p.Key == name select p )
			{
				int index = scriptTable.IndexOf ( pair );
				scriptTable [ index ] = scriptTable [ index + 1 ];
				scriptTable [ index + 1 ] = pair;
				break;
			}
		}

		public IEnumerator<KeyValuePair<string, string>> GetEnumerator ()
		{
			return scriptTable.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return scriptTable.GetEnumerator ();
		}
	}
}
