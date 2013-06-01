using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.IO
{
	public static class SubStreamHelper
	{
		public static SubStream CreateSubStream ( this Stream stream, int offset, int length )
		{
			return new SubStream ( stream, offset, length );
		}
	}

	public class SubStream : Stream
	{
		Stream baseStream;
		long start, len;
		long pos;

		public Stream BaseStream { get { return baseStream; } }

		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return baseStream.CanSeek; } }
		public override bool CanWrite { get { return false; } }
		public override bool CanTimeout { get { return baseStream.CanTimeout; } }

		public override int ReadTimeout
		{
			get { return baseStream.ReadTimeout; }
			set { throw new NotSupportedException (); }
		}

		public override int WriteTimeout
		{
			get { return baseStream.WriteTimeout; }
			set { throw new NotSupportedException (); }
		}

		public override void Flush ()
		{
			baseStream.Flush ();
		}

		public override long Position { get { return pos; } set { pos = value; } }
		public override long Length { get { return len; } }

		public override void SetLength ( long value )
		{
			len = value;
		}

		public SubStream ( Stream baseStream, long offset, long length )
		{
			if ( !baseStream.CanRead ) throw new ArgumentException ();

			this.baseStream = baseStream;
			start = offset;
			len = length;
		}

		public override long Seek ( long offset, SeekOrigin origin )
		{
			switch ( origin )
			{
				case SeekOrigin.Begin:
					pos = start + offset;
					break;
				case SeekOrigin.Current:
					pos += offset;
					break;
				case SeekOrigin.End:
					pos = len - offset;
					break;
			}

			return baseStream.Seek ( offset, origin );
		}

		public override int Read ( byte [] buffer, int offset, int count )
		{
			if ( baseStream.Position != pos + start )
				baseStream.Seek ( pos + start, SeekOrigin.Begin );

			if ( pos + count > len )
				count += ( int ) ( len - ( pos + count ) );

			int res = baseStream.Read ( buffer, offset, count );
			pos += res;

			return res;
		}

		public override int ReadByte ()
		{
			if ( pos + 1 > len ) return -1;
			else return baseStream.ReadByte ();
		}

		public override IAsyncResult BeginRead ( byte [] buffer, int offset, int count, AsyncCallback callback, object state )
		{
			throw new NotSupportedException ();
		}

		public override int EndRead ( IAsyncResult asyncResult )
		{
			throw new NotSupportedException ();
		}

		public override void Write ( byte [] buffer, int offset, int count )
		{
			throw new NotImplementedException ();
		}

		public override void WriteByte ( byte value )
		{
			throw new NotImplementedException ();
		}

		public override IAsyncResult BeginWrite ( byte [] buffer, int offset, int count, AsyncCallback callback, object state )
		{
			throw new NotImplementedException ();
		}

		public override void EndWrite ( IAsyncResult asyncResult )
		{
			throw new NotSupportedException ();
		}

		protected override void Dispose ( bool disposing )
		{

		}
	}
}
