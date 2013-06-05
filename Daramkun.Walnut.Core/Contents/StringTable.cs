using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Datas.Json;

namespace Daramkun.Walnut.Contents
{
	public sealed class StringTable
	{
		Dictionary<string, string> stringTable = new Dictionary<string, string> ();

		public StringTable ( Stream stream )
		{
			LoadFromJson ( JsonParser.Parse ( stream ), CultureInfo.CurrentCulture );
		}

		public StringTable ( Stream stream, CultureInfo cultureInfo )
		{
			LoadFromJson ( JsonParser.Parse ( stream ), cultureInfo );
		}

		public StringTable ( JsonEntry jsonEntry )
		{
			LoadFromJson ( jsonEntry, CultureInfo.CurrentCulture );
		}

		public StringTable ( JsonEntry jsonEntry, CultureInfo cultureInfo )
		{
			LoadFromJson ( jsonEntry, cultureInfo );
		}

		private void LoadFromJson ( JsonEntry jsonEntry, CultureInfo cultureInfo )
		{
			if ( jsonEntry [ "version" ].Data as string != "1.0" ) return;

			JsonEntry cultureEntry = null;
			if ( jsonEntry.Contains ( cultureInfo.EnglishName ) )
				cultureEntry = jsonEntry [ cultureInfo.EnglishName ].Data as JsonEntry;
			else if ( jsonEntry.Contains ( CultureInfo.InvariantCulture.TwoLetterISOLanguageName ) )
				cultureEntry = jsonEntry [ CultureInfo.InvariantCulture.TwoLetterISOLanguageName ].Data as JsonEntry;
			else return;

			foreach ( JsonItem item in cultureEntry )
			{
				stringTable.Add ( item.Name, item.Data as string );
			}
		}

		public string this [ string key ] { get { return stringTable [ key ]; } }

		public bool Contains ( string key ) { return stringTable.ContainsKey ( key ); }
	}
}
