using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Math
{
	public interface ICollision<T>
	{
		bool IsCollisionTo ( T obj );
	}
}
