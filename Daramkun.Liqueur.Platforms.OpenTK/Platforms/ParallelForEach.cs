using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramkun.Liqueur.Platforms
{
	public class ParallelForEach<T> : IForEach<T>
	{
		public void Run ( IEnumerable<T> iterator, Action<T> action )
		{
			Parallel.ForEach<T> ( iterator, action );
		}
	}
}
