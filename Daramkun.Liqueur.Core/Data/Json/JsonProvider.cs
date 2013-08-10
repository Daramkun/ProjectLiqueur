using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daramkun.Liqueur.Data.Json
{
	public static class JsonProvider
	{
		public static JsonEntry GetJsonEntry<T> ( T obj )
		{
			JsonEntry entry = new JsonEntry ();

			PropertyInfo [] props = typeof ( T ).GetProperties ();
			foreach ( PropertyInfo prop in props )
			{
				object [] attrs = prop.GetCustomAttributes ( typeof ( JsonItemAttribute ), true );
				foreach ( Attribute attr in attrs )
				{
					if ( attr is JsonItemAttribute )
					{
						string name = ( attr as JsonItemAttribute ).Name;
						object data = prop.GetValue ( obj, null );
						if ( name == null ) name = prop.Name;
						if ( data is Array )
						{
							object temp = data;
							data = new JsonArray ();
							foreach ( object value in ( temp as Array ) )
								( data as JsonArray ).Add ( value );
						}
						if ( data is IJsonEntry )
							data = ( data as IJsonEntry ).ToJsonEntry ();
						if ( data is IJsonArray )
							data = ( data as IJsonArray ).ToJsonArray ();
						entry.Add ( name, data );
					}
				}
			}

			FieldInfo [] fields = typeof ( T ).GetFields ();
			foreach ( FieldInfo field in fields )
			{
				object [] attrs = field.GetCustomAttributes ( typeof ( JsonItemAttribute ), true );
				foreach ( Attribute attr in attrs )
				{
					if ( attr is JsonItemAttribute )
					{
						string name = ( attr as JsonItemAttribute ).Name;
						object data = field.GetValue ( obj );
						if ( name == null ) name = field.Name;
						if ( data is Array )
						{
							object temp = data;
							data = new JsonArray ();
							foreach ( object value in ( temp as Array ) )
								( data as JsonArray ).Add ( value );
						}
						if ( data is IJsonEntry )
							data = ( data as IJsonEntry ).ToJsonEntry ();
						if ( data is IJsonArray )
							data = ( data as IJsonArray ).ToJsonArray ();
						entry.Add ( name, data );
					}
				}
			}

			return entry;
		}

		public static JsonArray GetJsonArray ( object obj, Type type )
		{
			if ( !( obj is IEnumerable ) ) throw new ArgumentException ();

			JsonArray array = new JsonArray ();
			foreach ( object item in ( obj as IEnumerable ) )
			{
				array.Add ( item );
			}

			return array;
		}
	}
}
