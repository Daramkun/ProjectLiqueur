using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Data.Json
{
	/// <summary>
	/// Json Entry class
	/// </summary>
	public sealed class JsonEntry : IEnumerable<KeyValuePair<string, object>>, IJsonEntry
	{
		Dictionary<string, object> jsonItemsByDic;

		/// <summary>
		/// Constructor of Json Entry
		/// </summary>
		public JsonEntry ()
		{
			jsonItemsByDic = new Dictionary<string, object> ();
		}

		IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator ()
		{
			return jsonItemsByDic.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return jsonItemsByDic.GetEnumerator ();
		}

		/// <summary>
		/// Json Item count
		/// </summary>
		public int Count { get { return jsonItemsByDic.Count; } }

		/// <summary>
		/// Get or set Json items
		/// </summary>
		/// <param name="name">Item Key</param>
		/// <returns>Item Value</returns>
		public object this [ string name ]
		{
			get { return jsonItemsByDic [ name ]; }
			set { jsonItemsByDic [ name ] = value; }
		}

		/// <summary>
		/// Is contains Json item?
		/// </summary>
		/// <param name="name">Item key</param>
		/// <returns>Contain state</returns>
		public bool Contains ( string name )
		{
			return jsonItemsByDic.ContainsKey ( name );
		}

		/// <summary>
		/// Add Json item
		/// </summary>
		/// <param name="name">Item key</param>
		/// <param name="item">Item value</param>
		/// <returns>Added item</returns>
		public object Add ( string name, object item )
		{
			jsonItemsByDic.Add ( name, item );
			return item;
		}
		
		/// <summary>
		/// Remove Json item
		/// </summary>
		/// <param name="name">Item key</param>
		public void Remove ( string name )
		{
			jsonItemsByDic.Remove ( name );
		}

		/// <summary>
		/// Generate Json string
		/// </summary>
		/// <returns>Json</returns>
		public override string ToString ()
		{
			string json = "{ ";

			foreach ( KeyValuePair<string, object> item in jsonItemsByDic )
			{
				string value;
				if ( item.Value is string ) value = string.Format ( "\"{0}\"", item.Value );
				else if ( item.Value is bool ) value = ( ( ( bool ) item.Value ) == true ) ? "true" : "false";
				else value = item.Value.ToString ();
				json += string.Format ( "\"{0}\" = {1}", item.Key, value ) + ", ";
			}

			return ( ( json == "{ " ) ? json : json.Substring ( 0, json.Length - 2 ) ) + " }";
		}

		/// <summary>
		/// Generate Json Entry
		/// </summary>
		/// <returns></returns>
		public JsonEntry ToJsonEntry ()
		{
			return this;
		}

		/// <summary>
		/// Configuration Json Entry from other Json Entry
		/// </summary>
		/// <param name="entry">other Json Entry</param>
		/// <returns>Self</returns>
		public object FromJsonEntry ( JsonEntry entry )
		{
			jsonItemsByDic = new Dictionary<string, object> ();
			foreach ( KeyValuePair<string, object> item in entry )
				Add ( item.Key, item.Value );
			return this;
		}
	}
}
