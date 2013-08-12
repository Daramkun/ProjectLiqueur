using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents.Decoder
{
	/// <summary>
	/// Decoder interface
	/// </summary>
	/// <typeparam name="T">Data structure</typeparam>
	public interface IDecoder<T>
	{
		/// <summary>
		/// Decode stream
		/// </summary>
		/// <param name="stream">Data stream</param>
		/// <param name="args">Argument, if you need</param>
		/// <returns>Decoded data</returns>
		T Decode ( Stream stream, params object [] args );
	}
}
