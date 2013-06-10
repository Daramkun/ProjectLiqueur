using System;
using System.IO;
using Daramkun.Liqueur.Exceptions;

namespace Daramkun.Liqueur.IO.Compression
{
	/// <summary>
	/// Deflate ½ºÆ®¸²
	/// </summary>
	public class DeflateStream : Stream
	{
		internal ZlibBaseStream baseStream;
		internal System.IO.Stream innerStream;
		bool disposed;

		public DeflateStream ( Stream stream, CompressionMode mode )
			: this ( stream, mode, CompressionLevel.Default, false )
		{
		}

		public DeflateStream ( Stream stream, CompressionMode mode, CompressionLevel level )
			: this ( stream, mode, level, false )
		{
		}

		public DeflateStream ( Stream stream, CompressionMode mode, bool leaveOpen )
			: this ( stream, mode, CompressionLevel.Default, leaveOpen )
		{
		}

		public DeflateStream ( Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen )
		{
			innerStream = stream;
			baseStream = new ZlibBaseStream ( stream, mode, level, ZlibStreamFlavor.DEFLATE, leaveOpen );
		}

		#region Zlib properties
		public virtual FlushType FlushMode
		{
			get { return ( this.baseStream.flushMode ); }
			set
			{
				if ( disposed ) throw new ObjectDisposedException ( "DeflateStream" );
				this.baseStream.flushMode = value;
			}
		}

		public int BufferSize
		{
			get
			{
				return this.baseStream._bufferSize;
			}
			set
			{
				if ( disposed ) throw new ObjectDisposedException ( "DeflateStream" );
				if ( this.baseStream._workingBuffer != null )
					throw new CompressionProcessException ( "The working buffer is already set." );
				if ( value < ZlibConstants.WorkingBufferSizeMin )
					throw new CompressionProcessException ( String.Format ( "Don't be silly. {0} bytes?? Use a bigger buffer, at least {1}.", value, ZlibConstants.WorkingBufferSizeMin ) );
				this.baseStream._bufferSize = value;
			}
		}

		public CompressionStrategy Strategy
		{
			get
			{
				return this.baseStream.Strategy;
			}
			set
			{
				if ( disposed ) throw new ObjectDisposedException ( "DeflateStream" );
				this.baseStream.Strategy = value;
			}
		}

		public virtual long TotalIn
		{
			get
			{
				return this.baseStream._z.TotalBytesIn;
			}
		}

		public virtual long TotalOut
		{
			get
			{
				return this.baseStream._z.TotalBytesOut;
			}
		}

		#endregion

		#region System.IO.Stream methods
		protected override void Dispose ( bool disposing )
		{
			try
			{
				if ( !disposed )
				{
					if ( disposing && ( this.baseStream != null ) )
						this.baseStream.Dispose ();
					disposed = true;
				}
			}
			finally
			{
				base.Dispose ( disposing );
			}
		}

		public override bool CanRead
		{
			get
			{
				if ( disposed ) throw new ObjectDisposedException ( "DeflateStream" );
				return baseStream.stream.CanRead;
			}
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get
			{
				if ( disposed ) throw new ObjectDisposedException ( "DeflateStream" );
				return baseStream.stream.CanWrite;
			}
		}

		public override void Flush ()
		{
			if ( disposed ) throw new ObjectDisposedException ( "DeflateStream" );
			baseStream.Flush ();
		}

		public override long Length { get { return -1; } }

		public override long Position
		{
			get
			{
				if ( this.baseStream.streamMode == ZlibBaseStream.StreamMode.Writer )
					return this.baseStream._z.TotalBytesOut;
				if ( this.baseStream.streamMode == ZlibBaseStream.StreamMode.Reader )
					return this.baseStream._z.TotalBytesIn;
				return 0;
			}
			set { throw new NotImplementedException (); }
		}

		public override int Read ( byte [] buffer, int offset, int count )
		{
			if ( disposed ) throw new ObjectDisposedException ( "DeflateStream" );
			return baseStream.Read ( buffer, offset, count );
		}

		public override long Seek ( long offset, System.IO.SeekOrigin origin )
		{
			throw new NotImplementedException ();
		}

		public override void SetLength ( long value )
		{
			throw new NotImplementedException ();
		}

		public override void Write ( byte [] buffer, int offset, int count )
		{
			if ( disposed ) throw new ObjectDisposedException ( "DeflateStream" );
			baseStream.Write ( buffer, offset, count );
		}
		#endregion

		public static byte [] CompressString ( string s )
		{
			using ( var ms = new MemoryStream () )
			{
				Stream compressor =
					new DeflateStream ( ms, CompressionMode.Compress, CompressionLevel.BestCompression );
				ZlibBaseStream.CompressString ( s, compressor );
				return ms.ToArray ();
			}
		}

		public static byte [] CompressBuffer ( byte [] b )
		{
			using ( var ms = new System.IO.MemoryStream () )
			{
				Stream compressor =
					new DeflateStream ( ms, CompressionMode.Compress, CompressionLevel.BestCompression );

				ZlibBaseStream.CompressBuffer ( b, compressor );
				return ms.ToArray ();
			}
		}

		public static String UncompressString ( byte [] compressed )
		{
			using ( var input = new System.IO.MemoryStream ( compressed ) )
			{
				Stream decompressor =
					new DeflateStream ( input, CompressionMode.Decompress );

				return ZlibBaseStream.UncompressString ( compressed, decompressor );
			}
		}

		public static byte [] UncompressBuffer ( byte [] compressed )
		{
			using ( var input = new System.IO.MemoryStream ( compressed ) )
			{
				Stream decompressor =
					new DeflateStream ( input, CompressionMode.Decompress );

				return ZlibBaseStream.UncompressBuffer ( compressed, decompressor );
			}
		}
	}
}
