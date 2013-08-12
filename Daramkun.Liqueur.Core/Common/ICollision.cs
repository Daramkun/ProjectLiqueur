using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Common
{
	/// <summary>
	/// Collision interface
	/// - geometry collision process
	/// </summary>
	/// <typeparam name="T">Geometry type</typeparam>
	public interface ICollision<T>
	{
		/// <summary>
		/// Is collision to an object
		/// </summary>
		/// <param name="obj">geometry object</param>
		/// <returns>Collision state</returns>
		bool IsCollisionTo ( T obj );
	}
}
