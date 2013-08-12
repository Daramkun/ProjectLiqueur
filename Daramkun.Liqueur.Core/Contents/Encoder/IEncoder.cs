using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents.Encoder
{
	/// <summary>
	/// Encoder interface
	/// </summary>
	/// <typeparam name="T">Data structure</typeparam>
	public interface IEncoder<T>
	{
		/// <summary>
		/// Encode stream
		/// </summary>
		/// <param name="stream">Destination Stream</param>
		/// <param name="data">Data</param>
		/// <param name="args">Argument, if you need</param>
		/// <returns>Encode state</returns>
		bool Encode ( Stream stream, T data, params object [] args );
	}
}
