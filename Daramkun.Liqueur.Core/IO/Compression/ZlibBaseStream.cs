using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.IO.Compression.Algorithms;
using Daramkun.Liqueur.IO.Compression.Utilities;

namespace Daramkun.Liqueur.IO.Compression
{
	internal class ZlibBaseStream : Stream
	{
		protected internal ZlibCodec _z = null;

		protected internal StreamMode streamMode = StreamMode.Undefined;
		protected internal FlushType flushMode;
		protected internal CompressionMode compressionMode;
		protected internal CompressionLevel level;
		protected internal bool leaveOpen;
		protected internal byte [] _workingBuffer;
		protected internal int _bufferSize = ZlibConstants.WorkingBufferSizeDefault;
		protected internal byte [] _buf1 = new byte [ 1 ];

		protected internal Stream stream;
		protected internal CompressionStrategy Strategy = CompressionStrategy.Default;

		public ZlibBaseStream ( Stream stream,
							  CompressionMode compressionMode,
							  CompressionLevel level,
							  bool leaveOpen )
			: base ()
		{
			this.flushMode = FlushType.None;
			this.stream = stream;
			this.leaveOpen = leaveOpen;
			this.compressionMode = compressionMode;
			this.level = level;
		}

		protected internal bool _wantCompress {　get {　return ( this.compressionMode == CompressionMode.Compress ); } }

		private ZlibCodec z
		{
			get
			{
				if ( _z == null )
				{
					bool wantRfc1950Header = false;
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

				}
				while ( !done );

				Flush ();
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

		public override long Seek ( long offset, SeekOrigin origin )
		{
			throw new NotImplementedException ();
		}

		public override void SetLength ( long value )
		{
			stream.SetLength ( value );
		}

		private bool nomoreinput = false;

		public override int Read ( byte [] buffer, int offset, int count )
		{
			if ( streamMode == StreamMode.Undefined )
			{
				if ( !this.stream.CanRead ) throw new CompressionProcessException ( "The stream is not readable." );
				streamMode = StreamMode.Reader;
				z.AvailableBytesIn = 0;
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

			return rc;
		}

		public override bool CanRead
		{
			get { return this.stream.CanRead; }
		}

		public override bool CanSeek
		{
			get { return this.stream.CanSeek; }
		}

		public override bool CanWrite
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

		public static void CompressString ( string s, Stream compressor )
		{
			byte [] uncompressed = Encoding.UTF8.GetBytes ( s );
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
