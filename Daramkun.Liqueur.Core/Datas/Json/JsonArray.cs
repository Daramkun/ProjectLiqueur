using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Datas.Json
{
	/// <summary>
	/// Json Array class
	/// </summary>
	public sealed class JsonArray : IEnumerable<JsonItem>, IJsonArray
	{
		List<JsonItem> jsonItems;

		/// <summary>
		/// Create JsonArray object
		/// </summary>
		public JsonArray ()
		{
			jsonItems = new List<JsonItem> ();
		}

		#region IEnumerable methods
		/// <summary>
		/// Get IEnumerator object
		/// </summary>
		/// <returns>IEnumerator object</returns>
		public IEnumerator<JsonItem> GetEnumerator ()
		{
			return jsonItems.GetEnumerator ();
		}

		IEnumerator IEnumerable.GetEnumerator ()
		{
			return jsonItems.GetEnumerator ();
		}
		#endregion

		/// <summary>
		/// Json Array items count
		/// </summary>
		public int Count { get { return jsonItems.Count; } }

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

		/// <summary>
		/// Add the Json Item
		/// </summary>
		/// <param name="item">JsonItem object</param>
		/// <returns>Incomming parameter</returns>
		public JsonItem Add ( JsonItem item )
		{
			jsonItems.Add ( item );
			return item;
		}

		/// <summary>
		/// Remove the Json Item
		/// </summary>
		/// <param name="item">JsonItem object</param>
		public void Remove ( JsonItem item )
		{
			jsonItems.Remove ( item );
		}

		/// <summary>
		/// Remove the Json Item
		/// </summary>
		/// <param name="index">JsonItem index</param>
		public void RemoveAt ( int index )
		{
			jsonItems.RemoveAt ( index );
		}

		/// <summary>
		/// Generate Json Array string
		/// </summary>
		/// <returns>Json Array string</returns>
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
		/// <summary>
		/// Make JsonArray from User class
		/// </summary>
		/// <returns>JsonArray object</returns>
		public JsonArray ToJsonArray ()
		{
			return this;
		}

		/// <summary>
		/// Analyze and apply to user class from JsonArray
		/// </summary>
		/// <param name="array">JsonArray object</param>
		/// <returns>this</returns>
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
