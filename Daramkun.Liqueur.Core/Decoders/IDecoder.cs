using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Decoders
{
	public interface IDecoder<T> where T : struct
	{
		T? Decode ( Stream stream );
	}
}
