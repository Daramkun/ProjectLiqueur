using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Datas.Json
{
	public sealed class JsonItem
	{
		string _name;
		object _data;

		public string Name { get { return _name; } set { _name = value; } }
		public object Data
		{
			get { return _data; }
			set
			{
				if ( value is string || value is int || value is float || value is bool ||
					value is double || value is short || value is byte || value is long ||
					value is uint || value is ushort || value is ulong || value is sbyte ||
					value == null )
					_data = value;
				else if ( value is IJsonArray )
					_data = ( value as IJsonArray ).ToJsonArray ();
				else if ( value is IJsonEntry )
					_data = ( value as IJsonEntry ).ToJsonEntry ();
				else if ( value is IEnumerable )
				{
					JsonArray array = new JsonArray ();
					foreach ( object obj in value as IEnumerable )
						array.Add ( new JsonItem ( null, obj ) );
					_data = array;
				}
				else if ( value is DateTime )
					_data = ( ( DateTime ) value ).ToString ();
				else if ( value is Enum )
					_data = ( int ) value;
				else
					throw new ArgumentException ( "This value is not json item type." );
			}
		}

		public JsonItem ( string name, object data )
		{
			Name = name;
			Data = data;
		}

		public override string ToString ()
		{
			string temp = ( Data == null ) ? "null" : ( ( Data is string ) ? Data as string : Data.ToString () );
			if ( Data is string )
			{
				temp = temp.Replace ( "\\", "\\\\" );
				temp = temp.Replace ( "\n", "\\n" );
				temp = temp.Replace ( "\"", "\\\"" );
				temp = String.Format ( "\"{0}\"", temp );
			}
			return String.Format ( "\"{0}\" : {1}", Name, temp);
		}
	}
}
