using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Data.Checksums
{
	public interface IChecksum<T>
	{
		T Result { get; }

		void Update ( int value );
		void Update ( Stream stream );
		void Update ( byte [] buffer );
		void Update ( byte [] buffer, int offset, int length );

		void Reset ();
	}
}
