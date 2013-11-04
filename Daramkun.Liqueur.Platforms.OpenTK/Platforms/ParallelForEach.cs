using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Daramkun.Liqueur.Platforms
{
	public class ParallelForEach : IForEach
	{
		public void Run ( IEnumerable<object> iterator, Action<object> action )
		{
			Parallel.ForEach<object> ( iterator, action );
		}
	}
}
