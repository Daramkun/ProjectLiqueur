using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Platforms
{
	public interface IForEach<T>
	{
		void Run ( IEnumerable<T> iterator, Action<T> action );
	}

	public class ForEach<T> : IForEach<T>
	{
		public void Run ( IEnumerable<T> iterator, Action<T> action )
		{
			foreach ( T item in iterator )
				action ( item );
		}
	}
}
