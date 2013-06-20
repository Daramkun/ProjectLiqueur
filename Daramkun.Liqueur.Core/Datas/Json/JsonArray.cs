using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Datas.Json
{
	public sealed class JsonArray : IEnumerable<JsonItem>, IJsonArray
	{
		List<JsonItem> jsonItems;

		public JsonArray ()
		{
			jsonItems = new List<JsonItem> ();
		}

		#region IEnumerable methods
		public IEnumerator<JsonItem> GetEnumerator ()
		{
			return jsonItems.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return jsonItems.GetEnumerator ();
		}
		#endregion

		public int Count { get { return jsonItems.Count; } }

		public JsonItem this [ int index ]
		{
			get { return jsonItems [ index ]; }
			set { jsonItems [ index ] = value; }
		}

		public JsonItem Add ( JsonItem item )
		{
			jsonItems.Add ( item );
			return item;
		}

		public void Remove ( JsonItem item )
		{
			jsonItems.Remove ( item );
		}

		public void RemoveAt ( int index )
		{
			jsonItems.RemoveAt ( index );
		}

		public override string ToString ()
		{
			string json = "[ ";

			foreach ( JsonItem item in jsonItems )
			{
				json += ( ( item.Data is string ) ? String.Format ( "\"{0}\"", item.Data ) : item.Data.ToString () ) + ", ";
			}

			return ( ( json == "[ " ) ? json : json.Substring ( 0, json.Length - 2 ) ) + " ]";
		}

		#region IJsonArray methods
		public JsonArray ToJsonArray ()
		{
			return this;
		}

		public object FromJsonArray ( JsonArray array )
		{
			jsonItems = new List<JsonItem> ();
			foreach ( JsonItem item in array )
				Add ( item );
			return this;
		}
		#endregion
	}
}
