using System;
using System.IO;
using Daramkun.Liqueur.Exceptions;

namespace Daramkun.Liqueur.IO.Compression
{
	class ZlibStream : Stream
	{
		internal ZlibBaseStream baseStream;
		bool disposed;

		public ZlibStream ( Stream stream, CompressionMode mode )
			: this ( stream, mode, CompressionLevel.Default, false )
		{
		}

		public ZlibStream ( Stream stream, CompressionMode mode, CompressionLevel level )
			: this ( stream, mode, level, false )
		{
		}

		public ZlibStream ( Stream stream, CompressionMode mode, bool leaveOpen )
			: this ( stream, mode, CompressionLevel.Default, leaveOpen )
		{
		}

		public ZlibStream ( Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen )
		{
			baseStream = new ZlibBaseStream ( stream, mode, level, ZlibStreamFlavor.ZLIB, leaveOpen );
		}

		#region Zlib properties
		public virtual FlushType FlushMode
		{
			get { return ( this.baseStream.flushMode ); }
			set
			{
				if ( disposed ) throw new ObjectDisposedException ( "ZlibStream" );
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
				if ( disposed ) throw new ObjectDisposedException ( "ZlibStream" );
				if ( this.baseStream._workingBuffer != null )
					throw new CompressionProcessException ( "The working buffer is already set." );
				if ( value < ZlibConstants.WorkingBufferSizeMin )
					throw new CompressionProcessException ( String.Format ( "Don't be silly. {0} bytes?? Use a bigger buffer, at least {1}.",
						value, ZlibConstants.WorkingBufferSizeMin ) );
				this.baseStream._bufferSize = value;
			}
		}

		public virtual long TotalIn
		{
			get { return this.baseStream._z.TotalBytesIn; }
		}

		public virtual long TotalOut
		{
			get { return this.baseStream._z.TotalBytesOut; }
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
				if ( disposed ) throw new ObjectDisposedException ( "ZlibStream" );
				return baseStream.stream.CanRead;
			}
		}

		public override bool CanSeek { get { return false; } }

		public override bool CanWrite
		{
			get
			{
				if ( disposed ) throw new ObjectDisposedException ( "ZlibStream" );
				return baseStream.stream.CanWrite;
			}
		}

		public override void Flush ()
		{
			if ( disposed ) throw new ObjectDisposedException ( "ZlibStream" );
			baseStream.Flush ();
		}

		public override long Length
		{
			get { throw new NotSupportedException (); }
		}

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
			set { throw new NotSupportedException (); }
		}

		public override int Read ( byte [] buffer, int offset, int count )
		{
			if ( disposed ) throw new ObjectDisposedException ( "ZlibStream" );
			return baseStream.Read ( buffer, offset, count );
		}

		public override long Seek ( long offset, System.IO.SeekOrigin origin )
		{
			throw new NotSupportedException ();
		}

		public override void SetLength ( long value )
		{
			throw new NotSupportedException ();
		}

		public override void Write ( byte [] buffer, int offset, int count )
		{
			if ( disposed ) throw new ObjectDisposedException ( "ZlibStream" );
			baseStream.Write ( buffer, offset, count );
		}
		#endregion

		public static byte [] CompressString ( String s )
		{
			using ( var ms = new MemoryStream () )
			{
				Stream compressor =
					new ZlibStream ( ms, CompressionMode.Compress, CompressionLevel.BestCompression );
				ZlibBaseStream.CompressString ( s, compressor );
				return ms.ToArray ();
			}
		}

		public static byte [] CompressBuffer ( byte [] b )
		{
			using ( var ms = new MemoryStream () )
			{
				Stream compressor =
					new ZlibStream ( ms, CompressionMode.Compress, CompressionLevel.BestCompression );

				ZlibBaseStream.CompressBuffer ( b, compressor );
				return ms.ToArray ();
			}
		}

		public static String UncompressString ( byte [] compressed )
		{
			using ( var input = new MemoryStream ( compressed ) )
			{
				Stream decompressor =
					new ZlibStream ( input, CompressionMode.Decompress );

				return ZlibBaseStream.UncompressString ( compressed, decompressor );
			}
		}

		public static byte [] UncompressBuffer ( byte [] compressed )
		{
			using ( var input = new MemoryStream ( compressed ) )
			{
				Stream decompressor =
					new ZlibStream ( input, CompressionMode.Decompress );

				return ZlibBaseStream.UncompressBuffer ( compressed, decompressor );
			}
		}
	}
}