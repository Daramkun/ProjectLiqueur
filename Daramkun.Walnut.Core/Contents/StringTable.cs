using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Datas.Json;

namespace Daramkun.Walnut.Contents
{
	public class StringTable
	{
		Dictionary<string, string> stringTable = new Dictionary<string, string> ();

		public StringTable ( Stream stream )
		{
			JsonEntry entry = JsonParser.Parse ( stream );
			foreach ( JsonItem item in entry )
				stringTable.Add ( item.Name, item.Data as string );
		}

		public StringTable ( JsonEntry jsonEntry )
		{
			foreach ( JsonItem item in jsonEntry )
				stringTable.Add ( item.Name, item.Data as string );
		}

		public string this [ string key ] { get { return stringTable [ key ]; } }
	}
}
