using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Graphics.Vertices;

namespace Daramkun.Liqueur.Graphics
{
	public class FlexibleVertexArray<T> : IEnumerable where T : IFlexibleVertex
	{
		T [] array;

		public event EventHandler<ArrayValueChangedEventArgs> ValueChanged;

		public FlexibleVertexArray ( int count )
		{
			array = new T [ count ];
		}

		public FlexibleVertexArray ( T [] baseArray )
		{
			array = new T [ baseArray.Length ];
			baseArray.CopyTo ( array, 0 );
		}

		public T this [ int index ]
		{
			get { return array [ index ]; }
			set
			{
				array [ index ] = value; if ( ValueChanged != null )
					ValueChanged ( this, new ArrayValueChangedEventArgs ( index ) );
			}
		}

		public int Length { get { return array.Length; } }

		public IEnumerator GetEnumerator ()
		{
			return array.GetEnumerator ();
		}

		public override string ToString ()
		{
			return string.Format ( "Flexible Vertex Array: {0}", array.Length );
		}
	}
}
