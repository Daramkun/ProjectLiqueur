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
	/// <summary>
	/// String Table class
	/// </summary>
	public sealed class StringTable
	{
		JsonEntry stringTable;

		/// <summary>
		/// Is Culture mode?
		/// (If this property true then String Table acquire LiqueurSystem.CurrentCulture, otherwise acquire Default culture)
		/// </summary>
		public bool IsCultureMode { get; set; }

		/// <summary>
		/// Constructor of String Table
		/// </summary>
		/// <param name="stream">String table stream</param>
		public StringTable ( Stream stream )
		{
			stringTable = JsonParser.Parse ( stream );
			if ( stringTable [ "tableversion" ] as string != "1.0" )
				throw new VersionMismatchException ();
		}

		/// <summary>
		/// Constructor of String Table
		/// </summary>
		/// <param name="jsonEntry">String table Json Entry</param>
		public StringTable ( JsonEntry jsonEntry )
		{
			stringTable = jsonEntry;
			if ( stringTable [ "tableversion" ] as string != "1.0" )
				throw new VersionMismatchException ();
		}

		/// <summary>
		/// Get or set string
		/// </summary>
		/// <param name="key">Table key</param>
		/// <returns>Table value</returns>
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

		/// <summary>
		/// Check contains key
		/// </summary>
		/// <param name="key">Table key</param>
		/// <returns>Contain state</returns>
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
