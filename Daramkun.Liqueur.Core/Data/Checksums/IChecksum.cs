using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Data.Checksums
{
	/// <summary>
	/// Checksum interface
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public interface IChecksum<T>
	{
		/// <summary>
		/// Checksum result
		/// </summary>
		T Result { get; }

		/// <summary>
		/// Update value
		/// </summary>
		/// <param name="value">value</param>
		void Update ( int value );
		/// <summary>
		/// Update stream
		/// </summary>
		/// <param name="stream">stream</param>
		void Update ( Stream stream );
		/// <summary>
		/// Update data
		/// </summary>
		/// <param name="buffer">data</param>
		void Update ( byte [] buffer );
		/// <summary>
		/// Update data
		/// </summary>
		/// <param name="buffer">data</param>
		/// <param name="offset">offset</param>
		/// <param name="length">length</param>
		void Update ( byte [] buffer, int offset, int length );

		/// <summary>
		/// Reset Checksum calculation
		/// </summary>
		void Reset ();
	}
}
