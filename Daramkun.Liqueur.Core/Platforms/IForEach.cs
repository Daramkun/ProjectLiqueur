using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Platforms
{
	public interface IForEach
	{
		void Run ( IEnumerable<object> iterator, Action<object> action );
	}

	public static class ForEachCollection
	{
		static Dictionary<string, IForEach> collection = new Dictionary<string, IForEach> ();

		public static void AddForEach ( string key, IForEach forEach ) { collection.Add ( key, forEach ); }

		public static IForEach GetForEach ( string key )
		{
			if ( collection.ContainsKey ( key ) ) return collection [ key ];
			else return null;
		}
	}
}
