using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Datas.Json
{
	/// <summary>
	/// Json Entry Interface
	/// </summary>
	public interface IJsonEntry
	{
		/// <summary>
		/// Make JsonEntry from User class
		/// </summary>
		/// <returns>JsonEntry object</returns>
		JsonEntry ToJsonEntry ();
		/// <summary>
		/// Analyze and apply to user class from JsonEntry
		/// </summary>
		/// <param name="entry">JsonEntry object</param>
		/// <returns>this</returns>
		object FromJsonEntry ( JsonEntry entry );
	}
}
