using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Datas.Json
{
	public sealed class JsonEntry : IEnumerable<JsonItem>, IJsonEntry
	{
		List<JsonItem> jsonItems;
		Dictionary<string, JsonItem> jsonItemsByDic;

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

		public int Count { get { return jsonItems.Count; } }

		public JsonItem this [ string name ]
		{
			get { return jsonItemsByDic [ name ]; }
			set { jsonItemsByDic [ name ] = value; }
		}

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

		public JsonItem Add ( JsonItem item )
		{
			jsonItems.Add ( item );
			jsonItemsByDic.Add ( item.Name, item );
			return item;
		}

		public void Remove ( string name )
		{
			jsonItems.Remove ( jsonItemsByDic [ name ] );
			jsonItemsByDic.Remove ( name );
		}

		public override string ToString ()
		{
			string json = "{ ";

			foreach ( JsonItem item in jsonItems )
			{
				json += item.ToString () + ", ";
			}

			return ( ( json == "{ " ) ? json : json.Substring ( 0, json.Length - 2 ) ) + " }";
		}

		public JsonEntry ToJsonEntry ()
		{
			return this;
		}

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
