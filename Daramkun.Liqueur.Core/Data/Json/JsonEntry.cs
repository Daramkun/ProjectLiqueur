using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Data.Json
{
	public sealed class JsonEntry : IEnumerable<KeyValuePair<string, object>>, IJsonEntry
	{
		Dictionary<string, object> jsonItemsByDic;

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

		public int Count { get { return jsonItemsByDic.Count; } }

		public object this [ string name ]
		{
			get { return jsonItemsByDic [ name ]; }
			set { jsonItemsByDic [ name ] = value; }
		}

		public bool Contains ( string name )
		{
			return jsonItemsByDic.ContainsKey ( name );
		}

		public object Add ( string name, object item )
		{
			jsonItemsByDic.Add ( name, item );
			return item;
		}

		public void Remove ( string name )
		{
			jsonItemsByDic.Remove ( name );
		}

		public override string ToString ()
		{
			string json = "{ ";

			foreach ( object item in jsonItemsByDic )
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
			jsonItemsByDic = new Dictionary<string, object> ();
			foreach ( KeyValuePair<string, object> item in entry )
				Add ( item.Key, item.Value );
			return this;
		}
	}
}
