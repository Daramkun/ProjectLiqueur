using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Datas.Json
{
	/// <summary>
	/// Json Array Interface
	/// </summary>
	public interface IJsonArray
	{
		/// <summary>
		/// Make JsonArray from User class
		/// </summary>
		/// <returns>JsonArray object</returns>
		JsonArray ToJsonArray ();
		/// <summary>
		/// Analyze and apply to user class from JsonArray
		/// </summary>
		/// <param name="array">JsonArray object</param>
		/// <returns>this</returns>
		object FromJsonArray ( JsonArray array );
	}
}
