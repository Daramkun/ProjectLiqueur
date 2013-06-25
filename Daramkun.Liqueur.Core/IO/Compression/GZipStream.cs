using System;
using System.IO;
using Daramkun.Liqueur.IO.Compression.Utilities;
using Daramkun.Liqueur.Exceptions;

namespace Daramkun.Liqueur.IO.Compression
{
	/// <summary>
	/// GZip ½ºÆ®¸²
	/// </summary>
	public class GZipStream : Stream
	{
		public string Comment
		{
			get
			{
				return comment;
			}
			set
			{
				if ( disposed ) throw new ObjectDisposedException ( "GZipStream" );
				comment = value;
			}
		}

		public string FileName
		{
			get { return fileName; }
			set
			{
				if ( disposed ) throw new ObjectDisposedException ( "GZipStream" );
				fileName = value;
				if ( fileName == null ) return;
				if ( fileName.IndexOf ( "/" ) != -1 )
				{
					fileName = fileName.Replace ( "/", "\\" );
				}
				if ( fileName.EndsWith ( "\\" ) )
					throw new Exception ( "Illegal filename" );
				if ( fileName.IndexOf ( "\\" ) != -1 )
				{
					fileName = Path2.GetFileName ( fileName );
				}
			}
		}

		public DateTime? LastModified;

		public int Crc32 { get { return crc32; } }

		private int headerByteCount;
		internal ZlibBaseStream baseStream;
		bool disposed;
		bool _firstReadDone;
		string fileName;
		string comment;
		int crc32;

		public GZipStream ( Stream stream, CompressionMode mode )
			: this ( stream, mode, CompressionLevel.Default, false )
		{
		}

		public GZipStream ( Stream stream, CompressionMode mode, CompressionLevel level )
			: this ( stream, mode, level, false )
		{
		}

		public GZipStream ( Stream stream, CompressionMode mode, bool leaveOpen )
			: this ( stream, mode, CompressionLevel.Default, leaveOpen )
		{
		}

		public GZipStream ( Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen )
		{
			baseStream = new ZlibBaseStream ( stream, mode, level, ZlibStreamFlavor.GZIP, leaveOpen );
		}

		#region Zlib properties
		public virtual FlushType FlushMode
		{
			get { return ( this.baseStream.flushMode ); }
			set
			{
				if ( disposed ) throw new ObjectDisposedException ( "GZipStream" );
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
				if ( disposed ) throw new ObjectDisposedException ( "GZipStream" );
				if ( this.baseStream._workingBuffer != null )
					throw new CompressionProcessException ( "The working buffer is already set." );
				if ( value < ZlibConstants.WorkingBufferSizeMin )
					throw new CompressionProcessException ( String.Format ( "Don't be silly. {0} bytes?? Use a bigger buffer, at least {1}.", value, ZlibConstants.WorkingBufferSizeMin ) );
				this.baseStream._bufferSize = value;
			}
		}

		public virtual long TotalIn { get { return this.baseStream._z.TotalBytesIn; } }
		public virtual long TotalOut { get { return this.baseStream._z.TotalBytesOut; } }

		#endregion

		#region Stream methods
		protected override void Dispose ( bool disposing )
		{
			try
			{
				if ( !disposed )
				{
					if ( disposing && ( this.baseStream != null ) )
					{
						this.baseStream.Dispose ();
						this.crc32 = baseStream.Crc32;
					}
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
				if ( disposed ) throw new ObjectDisposedException ( "GZipStream" );
				return baseStream.stream.CanRead;
			}
		}

		public override bool CanSeek { get { return false; } }

		public override bool CanWrite
		{
			get
			{
				if ( disposed ) throw new ObjectDisposedException ( "GZipStream" );
				return baseStream.stream.CanWrite;
			}
		}

		public override void Flush ()
		{
			if ( disposed ) throw new ObjectDisposedException ( "GZipStream" );
			baseStream.Flush ();
		}

		public override long Length { get { return -1; } }

		public override long Position
		{
			get
			{
				if ( this.baseStream.streamMode == ZlibBaseStream.StreamMode.Writer )
					return this.baseStream._z.TotalBytesOut + headerByteCount;
				if ( this.baseStream.streamMode == ZlibBaseStream.StreamMode.Reader )
					return this.baseStream._z.TotalBytesIn + this.baseStream._gzipHeaderByteCount;
				return 0;
			}

			set { throw new NotImplementedException (); }
		}

		public override int Read ( byte [] buffer, int offset, int count )
		{
			if ( disposed ) throw new ObjectDisposedException ( "GZipStream" );
			int n = baseStream.Read ( buffer, offset, count );

			if ( !_firstReadDone )
			{
				_firstReadDone = true;
				FileName = baseStream._GzipFileName;
				Comment = baseStream._GzipComment;
			}
			return n;
		}

		public override long Seek ( long offset, SeekOrigin origin )
		{
			throw new NotImplementedException ();
		}

		public override void SetLength ( long value )
		{
			throw new NotImplementedException ();
		}

		public override void Write ( byte [] buffer, int offset, int count )
		{
			if ( disposed ) throw new ObjectDisposedException ( "GZipStream" );
			if ( baseStream.streamMode == ZlibBaseStream.StreamMode.Undefined )
			{
				if ( baseStream._wantCompress ) headerByteCount = EmitHeader ();
				else throw new InvalidOperationException ();
			}

			baseStream.Write ( buffer, offset, count );
		}
		#endregion

		internal static readonly System.DateTime _unixEpoch = new System.DateTime ( 1970, 1, 1, 0, 0, 0, DateTimeKind.Utc );
//#if SILVERLIGHT || NETCF
        internal static readonly System.Text.Encoding iso8859dash1 = new Iso8859Dash1Encoding();
//#else
		//internal static readonly System.Text.Encoding iso8859dash1 = System.Text.Encoding.GetEncoding ( "iso-8859-1" );
//#endif

		private int EmitHeader ()
		{
			byte [] commentBytes = ( Comment == null ) ? null : iso8859dash1.GetBytes ( Comment );
			byte [] filenameBytes = ( FileName == null ) ? null : iso8859dash1.GetBytes ( FileName );

			int cbLength = ( Comment == null ) ? 0 : commentBytes.Length + 1;
			int fnLength = ( FileName == null ) ? 0 : filenameBytes.Length + 1;

			int bufferLength = 10 + cbLength + fnLength;
			byte [] header = new byte [ bufferLength ];
			int i = 0;
			header [ i++ ] = 0x1F;
			header [ i++ ] = 0x8B;

			header [ i++ ] = 8;
			byte flag = 0;
			if ( Comment != null )
				flag ^= 0x10;
			if ( FileName != null )
				flag ^= 0x8;

			header [ i++ ] = flag;

			if ( !LastModified.HasValue ) LastModified = DateTime.Now;
			System.TimeSpan delta = LastModified.Value - _unixEpoch;
			Int32 timet = ( Int32 ) delta.TotalSeconds;
			Array.Copy ( BitConverter.GetBytes ( timet ), 0, header, i, 4 );
			i += 4;

			header [ i++ ] = 0;
			header [ i++ ] = 0xFF;

			if ( fnLength != 0 )
			{
				Array.Copy ( filenameBytes, 0, header, i, fnLength - 1 );
				i += fnLength - 1;
				header [ i++ ] = 0;
			}

			if ( cbLength != 0 )
			{
				Array.Copy ( commentBytes, 0, header, i, cbLength - 1 );
				i += cbLength - 1;
				header [ i++ ] = 0;
			}

			baseStream.stream.Write ( header, 0, header.Length );

			return header.Length;
		}

		public static byte [] CompressString ( String s )
		{
			using ( var ms = new MemoryStream () )
			{
				Stream compressor = new GZipStream ( ms, CompressionMode.Compress, CompressionLevel.BestCompression );
				ZlibBaseStream.CompressString ( s, compressor );
				return ms.ToArray ();
			}
		}

		public static byte [] CompressBuffer ( byte [] b )
		{
			using ( var ms = new MemoryStream () )
			{
				Stream compressor = new GZipStream ( ms, CompressionMode.Compress, CompressionLevel.BestCompression );
				ZlibBaseStream.CompressBuffer ( b, compressor );
				return ms.ToArray ();
			}
		}

		public static String UncompressString ( byte [] compressed )
		{
			using ( var input = new MemoryStream ( compressed ) )
			{
				Stream decompressor = new GZipStream ( input, CompressionMode.Decompress );
				return ZlibBaseStream.UncompressString ( compressed, decompressor );
			}
		}

		public static byte [] UncompressBuffer ( byte [] compressed )
		{
			using ( var input = new System.IO.MemoryStream ( compressed ) )
			{
				Stream decompressor = new GZipStream ( input, CompressionMode.Decompress );
				return ZlibBaseStream.UncompressBuffer ( compressed, decompressor );
			}
		}
	}
}
