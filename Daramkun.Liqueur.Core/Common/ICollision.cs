using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Common
{
	public interface ICollision<T>
	{
		bool IsCollisionTo ( T obj );
	}
}
