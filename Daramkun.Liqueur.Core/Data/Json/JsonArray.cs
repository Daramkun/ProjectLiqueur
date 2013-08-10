using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Data.Json
{
	public sealed class JsonArray : IJsonArray
	{
		List<object> jsonItems;

		public JsonArray ()
		{
			jsonItems = new List<object> ();
		}

		#region IEnumerable methods
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return jsonItems.GetEnumerator ();
		}
		#endregion

		public int Count { get { return jsonItems.Count; } }

		public object this [ int index ]
		{
			get { return jsonItems [ index ]; }
			set { jsonItems [ index ] = value; }
		}

		public object Add ( object item )
		{
			jsonItems.Add ( item );
			return item;
		}

		public void Remove ( object item )
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

			foreach ( object item in jsonItems )
			{
				json += ( ( item is string ) ? String.Format ( "\"{0}\"", item ) : item.ToString () ) + ", ";
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
			jsonItems = new List<object> ();
			foreach ( object item in array )
				Add ( item );
			return this;
		}
		#endregion
	}
}
