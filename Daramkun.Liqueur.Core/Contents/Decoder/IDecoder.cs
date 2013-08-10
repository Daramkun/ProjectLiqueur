using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents.Decoder
{
	public interface IDecoder<T>
	{
		T Decode ( Stream stream, params object [] args );
	}
}
