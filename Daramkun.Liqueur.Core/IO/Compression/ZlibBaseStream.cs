using System;
using System.IO;
using Daramkun.Liqueur.IO.Compression.Crc;
using Daramkun.Liqueur.Exceptions;

namespace Daramkun.Liqueur.IO.Compression
{
	internal enum ZlibStreamFlavor { ZLIB = 1950, DEFLATE = 1951, GZIP = 1952 }

	internal class ZlibBaseStream : System.IO.Stream
	{
		protected internal ZlibCodec _z = null;

		protected internal StreamMode streamMode = StreamMode.Undefined;
		protected internal FlushType flushMode;
		protected internal ZlibStreamFlavor flavor;
		protected internal CompressionMode compressionMode;
		protected internal CompressionLevel level;
		protected internal bool leaveOpen;
		protected internal byte [] _workingBuffer;
		protected internal int _bufferSize = ZlibConstants.WorkingBufferSizeDefault;
		protected internal byte [] _buf1 = new byte [ 1 ];

		protected internal Stream stream;
		protected internal CompressionStrategy Strategy = CompressionStrategy.Default;

		CRC32 crc;
		protected internal string _GzipFileName;
		protected internal string _GzipComment;
		protected internal DateTime _GzipMtime;
		protected internal int _gzipHeaderByteCount;

		internal int Crc32 { get { if ( crc == null ) return 0; return crc.Crc32Result; } }

		public ZlibBaseStream ( System.IO.Stream stream,
							  CompressionMode compressionMode,
							  CompressionLevel level,
							  ZlibStreamFlavor flavor,
							  bool leaveOpen )
			: base ()
		{
			this.flushMode = FlushType.None;
			this.stream = stream;
			this.leaveOpen = leaveOpen;
			this.compressionMode = compressionMode;
			this.flavor = flavor;
			this.level = level;
			if ( flavor == ZlibStreamFlavor.GZIP )
			{
				this.crc = new CRC32 ();
			}
		}

		protected internal bool _wantCompress
		{
			get
			{
				return ( this.compressionMode == CompressionMode.Compress );
			}
		}

		private ZlibCodec z
		{
			get
			{
				if ( _z == null )
				{
					bool wantRfc1950Header = ( this.flavor == ZlibStreamFlavor.ZLIB );
					_z = new ZlibCodec ();
					if ( this.compressionMode == CompressionMode.Decompress )
					{
						_z.InitializeInflate ( wantRfc1950Header );
					}
					else
					{
						_z.Strategy = Strategy;
						_z.InitializeDeflate ( this.level, wantRfc1950Header );
					}
				}
				return _z;
			}
		}

		private byte [] workingBuffer
		{
			get
			{
				if ( _workingBuffer == null )
					_workingBuffer = new byte [ _bufferSize ];
				return _workingBuffer;
			}
		}

		public override void Write ( System.Byte [] buffer, int offset, int count )
		{
			if ( crc != null )
				crc.SlurpBlock ( buffer, offset, count );

			if ( streamMode == StreamMode.Undefined )
				streamMode = StreamMode.Writer;
			else if ( streamMode != StreamMode.Writer )
				throw new CompressionProcessException ( "Cannot Write after Reading." );

			if ( count == 0 )
				return;

			z.InputBuffer = buffer;
			_z.NextIn = offset;
			_z.AvailableBytesIn = count;
			bool done = false;
			do
			{
				_z.OutputBuffer = workingBuffer;
				_z.NextOut = 0;
				_z.AvailableBytesOut = _workingBuffer.Length;
				int rc = ( _wantCompress )
					? _z.Deflate ( flushMode )
					: _z.Inflate ( flushMode );
				if ( rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END )
					throw new CompressionProcessException ( ( _wantCompress ? "de" : "in" ) + "flating: " + _z.Message );

				stream.Write ( _workingBuffer, 0, _workingBuffer.Length - _z.AvailableBytesOut );

				done = _z.AvailableBytesIn == 0 && _z.AvailableBytesOut != 0;

				if ( flavor == ZlibStreamFlavor.GZIP && !_wantCompress )
					done = ( _z.AvailableBytesIn == 8 && _z.AvailableBytesOut != 0 );

			}
			while ( !done );
		}

		private void finish ()
		{
			if ( _z == null ) return;

			if ( streamMode == StreamMode.Writer )
			{
				bool done = false;
				do
				{
					_z.OutputBuffer = workingBuffer;
					_z.NextOut = 0;
					_z.AvailableBytesOut = _workingBuffer.Length;
					int rc = ( _wantCompress )
						? _z.Deflate ( FlushType.Finish )
						: _z.Inflate ( FlushType.Finish );

					if ( rc != ZlibConstants.Z_STREAM_END && rc != ZlibConstants.Z_OK )
					{
						string verb = ( _wantCompress ? "de" : "in" ) + "flating";
						if ( _z.Message == null )
							throw new CompressionProcessException ( String.Format ( "{0}: (rc = {1})", verb, rc ) );
						else
							throw new CompressionProcessException ( verb + ": " + _z.Message );
					}

					if ( _workingBuffer.Length - _z.AvailableBytesOut > 0 )
					{
						stream.Write ( _workingBuffer, 0, _workingBuffer.Length - _z.AvailableBytesOut );
					}

					done = _z.AvailableBytesIn == 0 && _z.AvailableBytesOut != 0;
					if ( flavor == ZlibStreamFlavor.GZIP && !_wantCompress )
						done = ( _z.AvailableBytesIn == 8 && _z.AvailableBytesOut != 0 );

				}
				while ( !done );

				Flush ();

				if ( flavor == ZlibStreamFlavor.GZIP )
				{
					if ( _wantCompress )
					{
						int c1 = crc.Crc32Result;
						stream.Write ( BitConverter.GetBytes ( c1 ), 0, 4 );
						int c2 = ( Int32 ) ( crc.TotalBytesRead & 0x00000000FFFFFFFF );
						stream.Write ( BitConverter.GetBytes ( c2 ), 0, 4 );
					}
					else
					{
						throw new CompressionProcessException ( "Writing with decompression is not supported." );
					}
				}
			}
			else if ( streamMode == StreamMode.Reader )
			{
				if ( flavor == ZlibStreamFlavor.GZIP )
				{
					if ( !_wantCompress )
					{
						if ( _z.TotalBytesOut == 0L )
							return;

						byte [] trailer = new byte [ 8 ];

						if ( _z.AvailableBytesIn < 8 )
						{
							Array.Copy ( _z.InputBuffer, _z.NextIn, trailer, 0, _z.AvailableBytesIn );
							int bytesNeeded = 8 - _z.AvailableBytesIn;
							int bytesRead = stream.Read ( trailer,
														 _z.AvailableBytesIn,
														 bytesNeeded );
							if ( bytesNeeded != bytesRead )
							{
								throw new CompressionProcessException ( String.Format ( "Missing or incomplete GZIP trailer. Expected 8 bytes, got {0}.",
																	  _z.AvailableBytesIn + bytesRead ) );
							}
						}
						else
						{
							Array.Copy ( _z.InputBuffer, _z.NextIn, trailer, 0, trailer.Length );
						}

						Int32 crc32_expected = BitConverter.ToInt32 ( trailer, 0 );
						Int32 crc32_actual = crc.Crc32Result;
						Int32 isize_expected = BitConverter.ToInt32 ( trailer, 4 );
						Int32 isize_actual = ( Int32 ) ( _z.TotalBytesOut & 0x00000000FFFFFFFF );

						if ( crc32_actual != crc32_expected )
							throw new CompressionProcessException ( String.Format ( "Bad CRC32 in GZIP trailer. (actual({0:X8})!=expected({1:X8}))", crc32_actual, crc32_expected ) );

						if ( isize_actual != isize_expected )
							throw new CompressionProcessException ( String.Format ( "Bad size in GZIP trailer. (actual({0})!=expected({1}))", isize_actual, isize_expected ) );

					}
					else
					{
						throw new CompressionProcessException ( "Reading with compression is not supported." );
					}
				}
			}
		}

		private void end ()
		{
			if ( z == null )
				return;
			if ( _wantCompress )
			{
				_z.EndDeflate ();
			}
			else
			{
				_z.EndInflate ();
			}
			_z = null;
		}

		protected override void Dispose ( bool isDisposed )
		{
			if ( isDisposed )
			{
				if ( stream == null ) return;
				try
				{
					finish ();
				}
				finally
				{
					end ();
					if ( !leaveOpen ) stream.Dispose ();
					stream = null;
				}
			}
		}

		public override void Flush ()
		{
			stream.Flush ();
		}

		public override System.Int64 Seek ( System.Int64 offset, System.IO.SeekOrigin origin )
		{
			throw new NotImplementedException ();
		}

		public override void SetLength ( System.Int64 value )
		{
			stream.SetLength ( value );
		}

		private bool nomoreinput = false;

		private string ReadZeroTerminatedString ()
		{
			var list = new System.Collections.Generic.List<byte> ();
			bool done = false;
			do
			{
				int n = stream.Read ( _buf1, 0, 1 );
				if ( n != 1 )
					throw new CompressionProcessException ( "Unexpected EOF reading GZIP header." );
				else
				{
					if ( _buf1 [ 0 ] == 0 )
						done = true;
					else
						list.Add ( _buf1 [ 0 ] );
				}
			} while ( !done );
			byte [] a = list.ToArray ();
			return GZipStream.iso8859dash1.GetString ( a, 0, a.Length );
		}

		private int _ReadAndValidateGzipHeader ()
		{
			int totalBytesRead = 0;
			byte [] header = new byte [ 10 ];
			int n = stream.Read ( header, 0, header.Length );

			if ( n == 0 )
				return 0;

			if ( n != 10 )
				throw new CompressionProcessException ( "Not a valid GZIP stream." );

			if ( header [ 0 ] != 0x1F || header [ 1 ] != 0x8B || header [ 2 ] != 8 )
				throw new CompressionProcessException ( "Bad GZIP header." );

			Int32 timet = BitConverter.ToInt32 ( header, 4 );
			_GzipMtime = GZipStream._unixEpoch.AddSeconds ( timet );
			totalBytesRead += n;
			if ( ( header [ 3 ] & 0x04 ) == 0x04 )
			{
				n = stream.Read ( header, 0, 2 );
				totalBytesRead += n;

				Int16 extraLength = ( Int16 ) ( header [ 0 ] + header [ 1 ] * 256 );
				byte [] extra = new byte [ extraLength ];
				n = stream.Read ( extra, 0, extra.Length );
				if ( n != extraLength )
					throw new CompressionProcessException ( "Unexpected end-of-file reading GZIP header." );
				totalBytesRead += n;
			}
			if ( ( header [ 3 ] & 0x08 ) == 0x08 )
				_GzipFileName = ReadZeroTerminatedString ();
			if ( ( header [ 3 ] & 0x10 ) == 0x010 )
				_GzipComment = ReadZeroTerminatedString ();
			if ( ( header [ 3 ] & 0x02 ) == 0x02 )
				Read ( _buf1, 0, 1 );

			return totalBytesRead;
		}

		public override System.Int32 Read ( System.Byte [] buffer, System.Int32 offset, System.Int32 count )
		{
			if ( streamMode == StreamMode.Undefined )
			{
				if ( !this.stream.CanRead ) throw new CompressionProcessException ( "The stream is not readable." );
				streamMode = StreamMode.Reader;
				z.AvailableBytesIn = 0;
				if ( flavor == ZlibStreamFlavor.GZIP )
				{
					_gzipHeaderByteCount = _ReadAndValidateGzipHeader ();
					if ( _gzipHeaderByteCount == 0 )
						return 0;
				}
			}

			if ( streamMode != StreamMode.Reader )
				throw new CompressionProcessException ( "Cannot Read after Writing." );

			if ( count == 0 ) return 0;
			if ( nomoreinput && _wantCompress ) return 0;
			if ( buffer == null ) throw new ArgumentNullException ( "buffer" );
			if ( count < 0 ) throw new ArgumentOutOfRangeException ( "count" );
			if ( offset < buffer.GetLowerBound ( 0 ) ) throw new ArgumentOutOfRangeException ( "offset" );
			if ( ( offset + count ) > buffer.GetLength ( 0 ) ) throw new ArgumentOutOfRangeException ( "count" );

			int rc = 0;

			_z.OutputBuffer = buffer;
			_z.NextOut = offset;
			_z.AvailableBytesOut = count;

			_z.InputBuffer = workingBuffer;

			do
			{
				if ( ( _z.AvailableBytesIn == 0 ) && ( !nomoreinput ) )
				{
					_z.NextIn = 0;
					_z.AvailableBytesIn = stream.Read ( _workingBuffer, 0, _workingBuffer.Length );
					if ( _z.AvailableBytesIn == 0 )
						nomoreinput = true;

				}
				rc = ( _wantCompress )
					? _z.Deflate ( flushMode )
					: _z.Inflate ( flushMode );

				if ( nomoreinput && ( rc == ZlibConstants.Z_BUF_ERROR ) )
					return 0;

				if ( rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END )
					throw new CompressionProcessException ( String.Format ( "{0}flating:  rc={1}  msg={2}", ( _wantCompress ? "de" : "in" ), rc, _z.Message ) );

				if ( ( nomoreinput || rc == ZlibConstants.Z_STREAM_END ) && ( _z.AvailableBytesOut == count ) )
					break;
			}
			while ( _z.AvailableBytesOut > 0 && !nomoreinput && rc == ZlibConstants.Z_OK );

			if ( _z.AvailableBytesOut > 0 )
			{
				if ( rc == ZlibConstants.Z_OK && _z.AvailableBytesIn == 0 )
				{

				}

				if ( nomoreinput )
				{
					if ( _wantCompress )
					{
						rc = _z.Deflate ( FlushType.Finish );

						if ( rc != ZlibConstants.Z_OK && rc != ZlibConstants.Z_STREAM_END )
							throw new CompressionProcessException ( String.Format ( "Deflating:  rc={0}  msg={1}", rc, _z.Message ) );
					}
				}
			}


			rc = ( count - _z.AvailableBytesOut );

			if ( crc != null )
				crc.SlurpBlock ( buffer, offset, rc );

			return rc;
		}

		public override System.Boolean CanRead
		{
			get { return this.stream.CanRead; }
		}

		public override System.Boolean CanSeek
		{
			get { return this.stream.CanSeek; }
		}

		public override System.Boolean CanWrite
		{
			get { return this.stream.CanWrite; }
		}

		public override System.Int64 Length
		{
			get { return stream.Length; }
		}

		public override long Position
		{
			get { throw new NotImplementedException (); }
			set { throw new NotImplementedException (); }
		}

		internal enum StreamMode
		{
			Writer,
			Reader,
			Undefined,
		}


		public static void CompressString ( String s, Stream compressor )
		{
			byte [] uncompressed = System.Text.Encoding.UTF8.GetBytes ( s );
			using ( compressor )
			{
				compressor.Write ( uncompressed, 0, uncompressed.Length );
			}
		}

		public static void CompressBuffer ( byte [] b, Stream compressor )
		{
			using ( compressor )
			{
				compressor.Write ( b, 0, b.Length );
			}
		}

		public static String UncompressString ( byte [] compressed, Stream decompressor )
		{
			byte [] working = new byte [ 1024 ];
			var encoding = System.Text.Encoding.UTF8;
			using ( var output = new MemoryStream () )
			{
				using ( decompressor )
				{
					int n;
					while ( ( n = decompressor.Read ( working, 0, working.Length ) ) != 0 )
					{
						output.Write ( working, 0, n );
					}
				}

				output.Seek ( 0, SeekOrigin.Begin );
				var sr = new StreamReader ( output, encoding );
				return sr.ReadToEnd ();
			}
		}

		public static byte [] UncompressBuffer ( byte [] compressed, Stream decompressor )
		{
			byte [] working = new byte [ 1024 ];
			using ( var output = new MemoryStream () )
			{
				using ( decompressor )
				{
					int n;
					while ( ( n = decompressor.Read ( working, 0, working.Length ) ) != 0 )
					{
						output.Write ( working, 0, n );
					}
				}
				return output.ToArray ();
			}
		}
	}
}
