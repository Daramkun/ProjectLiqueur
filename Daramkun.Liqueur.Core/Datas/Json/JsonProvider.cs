using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Daramkun.Liqueur.Datas.Json
{
	/// <summary>
	/// Making JsonEntry or JsonArray from User classes
	/// </summary>
	public static class JsonProvider
	{
		/// <summary>
		/// Generate JsonEntry from User class
		/// </summary>
		/// <param name="obj">User class object</param>
		/// <param name="type">User class type</param>
		/// <returns>JsonEntry object</returns>
		public static JsonEntry GetJsonEntry ( object obj, Type type )
		{
			JsonEntry entry = new JsonEntry ();

			PropertyInfo [] props = type.GetProperties ();
			foreach ( PropertyInfo prop in props )
			{
				object [] attrs = prop.GetCustomAttributes ( typeof ( JsonItembleAttribute ), true );

				foreach ( Attribute attr in attrs )
				{
					if ( attr is JsonItembleAttribute )
					{
						string name = ( attr as JsonItembleAttribute ).Name;
						object data = prop.GetValue ( obj, null );
						if ( name == null ) name = prop.Name;
						entry.Add ( new JsonItem ( name, data ) );
					}
				}
			}

			return entry;
		}

		/// <summary>
		/// Generate JsonArray from User class
		/// </summary>
		/// <param name="obj">User class object</param>
		/// <param name="type">User class type</param>
		/// <returns>JsonArray object</returns>
		public static JsonArray GetJsonArray ( object obj, Type type )
		{
			JsonArray array = new JsonArray ();

			PropertyInfo [] props = type.GetProperties ();
			foreach ( PropertyInfo prop in props )
			{
				object [] attrs = prop.GetCustomAttributes ( typeof ( JsonItembleAttribute ), true );

				foreach ( Attribute attr in attrs )
				{
					if ( attr is JsonItembleAttribute )
					{
						string name = ( attr as JsonItembleAttribute ).Name;
						object data = prop.GetValue ( obj, null );
						if ( name == null ) name = prop.Name;
						array.Add ( new JsonItem ( name, data ) );
					}
				}
			}

			return array;
		}
	}
}
