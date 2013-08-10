using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Data.Json;
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
			if ( stringTable [ "tableversion" ] as string != "1.0" )
				throw new VersionMismatchException ();
		}

		public StringTable ( JsonEntry jsonEntry )
		{
			stringTable = jsonEntry;
			if ( stringTable [ "tableversion" ] as string != "1.0" )
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
						JsonEntry innerEntry = stringTable [ LiqueurSystem.CurrentCulture.Name ] as JsonEntry;
						if ( innerEntry.Contains ( key ) ) return innerEntry [ key ] as string;
					}
				}
				if ( !stringTable.Contains ( key ) ) return null;
				return stringTable [ key ] as string;
			}
		}

		public bool Contains ( string key )
		{
			if ( IsCultureMode )
			{
				if ( stringTable.Contains ( CultureInfo.CurrentCulture.Name ) )
				{
					bool result = ( stringTable [ LiqueurSystem.CurrentCulture.Name ] as JsonEntry ).Contains ( key );
					if ( result ) return result;
				}
			}
			return stringTable.Contains ( key );
		}
	}
}
