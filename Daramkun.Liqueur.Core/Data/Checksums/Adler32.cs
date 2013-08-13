using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Data.Checksums
{
	/// <summary>
	/// Adler32 Checksum class
	/// </summary>
	public class Adler32 : IChecksum<uint>
	{
		/// <summary>
		/// Checksum result
		/// </summary>
		public uint Result { get; private set; }

		/// <summary>
		/// Constructor of Adler32 Checksum
		/// </summary>
		public Adler32 ()
		{
			Reset ();
		}

		/// <summary>
		/// Update value
		/// </summary>
		/// <param name="value">value</param>
		public void Update ( int value )
		{
			uint s1 = ( ( Result & 0xffff ) + ( ( uint ) value & 0xff ) ) % 65521;
			uint s2 = ( s1 + ( Result >> 16 ) ) % 65521;
			Result = ( s2 << 16 ) + s1;
		}

		/// <summary>
		/// Update stream
		/// </summary>
		/// <param name="stream">stream</param>
		public void Update ( Stream stream )
		{
			byte [] buffer = new byte [ stream.Length ];
			stream.Read ( buffer, 0, buffer.Length );
			Update ( buffer );
		}

		/// <summary>
		/// Update data
		/// </summary>
		/// <param name="buffer">data</param>
		public void Update ( byte [] buffer )
		{
			Update ( buffer, 0, buffer.Length );
		}

		/// <summary>
		/// Update data
		/// </summary>
		/// <param name="buffer">data</param>
		/// <param name="offset">offset</param>
		/// <param name="length">length</param>
		public void Update ( byte [] buffer, int offset, int length )
		{
			if ( buffer == null )
				throw new ArgumentNullException ();
			if ( length < 0 || offset + length > buffer.Length )
				throw new ArgumentOutOfRangeException ();

			uint s1 = Result & 0xffff;
			uint s2 = Result >> 16;

			while ( length > 0 )
			{
				int n = 3800;
				if ( n > length )
					n = length;
				length -= n;
				while ( --n >= 0 )
				{
					s1 = s1 + ( uint ) ( buffer [ offset++ ] & 0xff );
					s2 = s2 + s1;
				}
				s1 %= 65521;
				s2 %= 65521;
			}

			Result = ( s2 << 16 ) | s1;
		}

		/// <summary>
		/// Reset Checksum calculation
		/// </summary>
		public void Reset ()
		{
			Result = 1;
		}
	}
}
