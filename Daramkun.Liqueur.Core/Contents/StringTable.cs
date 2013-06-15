using System;
using System.Globalization;
using System.IO;
using System.Text;
using Daramkun.Liqueur.Datas.Json;
using Daramkun.Liqueur.Exceptions;

namespace Daramkun.Liqueur.Contents
{
	public sealed class StringTable
	{
		JsonEntry stringTable;

		public bool IsCultureMode { get; set; }

		public StringTable ( Stream stream )
		{
			stringTable = JsonParser.Parse ( stream );
			if ( stringTable [ "tableversion" ].Data as string != "1.0" )
				throw new VersionMismatchException ();
		}

		public StringTable ( JsonEntry jsonEntry )
		{
			stringTable = jsonEntry;
			if ( stringTable [ "tableversion" ].Data as string != "1.0" )
				throw new VersionMismatchException ();
		}

		public string this [ string key ]
		{
			get
			{
				if ( IsCultureMode )
				{
					if ( stringTable.Contains ( LiqueurSystem.CurrentCulture.Name ) )
					{
						JsonEntry innerEntry = stringTable [ LiqueurSystem.CurrentCulture.Name ].Data as JsonEntry;
						if ( innerEntry.Contains ( key ) ) return innerEntry [ key ].Data as string;
					}
				}
				if ( !stringTable.Contains ( key ) ) return null;
				return stringTable [ key ].Data as string;
			}
		}

		public bool Contains ( string key )
		{
			if ( IsCultureMode )
			{
				if ( stringTable.Contains ( CultureInfo.CurrentCulture.Name ) )
				{
					bool result = ( stringTable [ LiqueurSystem.CurrentCulture.Name ].Data as JsonEntry ).Contains ( key );
					if ( result ) return result;
				}
			}
			return stringTable.Contains ( key );
		}
	}
}
