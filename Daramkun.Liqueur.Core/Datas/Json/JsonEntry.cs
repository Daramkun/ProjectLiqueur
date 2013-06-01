using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Datas.Json
{
	/// <summary>
	/// Json Entry class
	/// </summary>
	public sealed class JsonEntry : IEnumerable<JsonItem>, IJsonEntry
	{
		List<JsonItem> jsonItems;
		Dictionary<string, JsonItem> jsonItemsByDic;

		/// <summary>
		/// Create JsonEntry object
		/// </summary>
		public JsonEntry ()
		{
			jsonItems = new List<JsonItem> ();
			jsonItemsByDic = new Dictionary<string, JsonItem> ();
		}

		IEnumerator<JsonItem> IEnumerable<JsonItem>.GetEnumerator ()
		{
			return jsonItems.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return jsonItems.GetEnumerator ();
		}

		/// <summary>
		/// Json Entry items count
		/// </summary>
		public int Count { get { return jsonItems.Count; } }

		/// <summary>
		/// Get or set the JsonItems
		/// </summary>
		/// <param name="name">JsonItem name</param>
		/// <returns>JsonItem object</returns>
		public JsonItem this [ string name ]
		{
			get { return jsonItemsByDic [ name ]; }
			set { jsonItemsByDic [ name ] = value; }
		}

		/// <summary>
		/// Get or set the JsonItems
		/// </summary>
		/// <param name="index">JsonItem index</param>
		/// <returns>JsonItem object</returns>
		public JsonItem this [ int index ]
		{
			get { return jsonItems [ index ]; }
			set { jsonItems [ index ] = value; }
		}

		public bool Contains ( string name )
		{
			return jsonItemsByDic.ContainsKey ( name );
		}

		public bool Contains ( int index )
		{
			return !( jsonItems.Count <= index );
		}

		/// <summary>
		/// Add the Json Item
		/// </summary>
		/// <param name="item">JsonItem object</param>
		/// <returns>Incomming parameter</returns>
		public JsonItem Add ( JsonItem item )
		{
			jsonItems.Add ( item );
			jsonItemsByDic.Add ( item.Name, item );
			return item;
		}

		/// <summary>
		/// Remove the Json Item
		/// </summary>
		/// <param name="name">JsonItem name</param>
		public void Remove ( string name )
		{
			jsonItems.Remove ( jsonItemsByDic [ name ] );
			jsonItemsByDic.Remove ( name );
		}

		/// <summary>
		/// Generate Json string
		/// </summary>
		/// <returns>Json string</returns>
		public override string ToString ()
		{
			string json = "{ ";

			foreach ( JsonItem item in jsonItems )
			{
				json += item.ToString () + ", ";
			}

			return ( ( json == "{ " ) ? json : json.Substring ( 0, json.Length - 2 ) ) + " }";
		}

		/// <summary>
		/// Make JsonEntry from User class
		/// </summary>
		/// <returns>JsonEntry object</returns>
		public JsonEntry ToJsonEntry ()
		{
			return this;
		}

		/// <summary>
		/// Analyze and apply to user class from JsonEntry
		/// </summary>
		/// <param name="entry">JsonEntry object</param>
		/// <returns>this</returns>
		public object FromJsonEntry ( JsonEntry entry )
		{
			jsonItems = new List<JsonItem> ();
			jsonItemsByDic = new Dictionary<string, JsonItem> ();
			foreach ( JsonItem item in entry )
				Add ( item );
			return this;
		}
	}
}
