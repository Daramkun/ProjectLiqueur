using System;
using System.Runtime.InteropServices;
using Daramkun.Liqueur.IO.Compression.Algorithms;
using Daramkun.Liqueur.Exceptions;

namespace Daramkun.Liqueur.IO.Compression
{
	sealed class ZlibCodec
	{
		public byte [] InputBuffer;
		public int NextIn;
		public int AvailableBytesIn;
		public long TotalBytesIn;
		public byte [] OutputBuffer;
		public int NextOut;
		public int AvailableBytesOut;
		public long TotalBytesOut;
		public string Message;

		internal DeflateManager dstate;
		internal InflateManager istate;

		internal uint adler32;

		public CompressionLevel CompressLevel = CompressionLevel.Default;
		public int WindowBits = ZlibConstants.WindowBitsDefault;
		public CompressionStrategy Strategy = CompressionStrategy.Default;
		public int Adler32 { get { return ( int ) adler32; } }

		public ZlibCodec () { }

		public ZlibCodec ( CompressionMode mode )
		{
			if ( mode == CompressionMode.Compress )
			{
				int rc = InitializeDeflate ();
				if ( rc != ZlibConstants.Z_OK ) throw new CompressionProcessException ( "Cannot initialize for deflate." );
			}
			else if ( mode == CompressionMode.Decompress )
			{
				int rc = InitializeInflate ();
				if ( rc != ZlibConstants.Z_OK ) throw new CompressionProcessException ( "Cannot initialize for inflate." );
			}
			else throw new CompressionProcessException ( "Invalid ZlibStreamFlavor." );
		}

		public int InitializeInflate ()
		{
			return InitializeInflate ( this.WindowBits );
		}

		public int InitializeInflate ( bool expectRfc1950Header )
		{
			return InitializeInflate ( this.WindowBits, expectRfc1950Header );
		}

		public int InitializeInflate ( int windowBits )
		{
			this.WindowBits = windowBits;
			return InitializeInflate ( windowBits, true );
		}

		public int InitializeInflate ( int windowBits, bool expectRfc1950Header )
		{
			this.WindowBits = windowBits;
			if ( dstate != null ) throw new CompressionProcessException ( "You may not call InitializeInflate() after calling InitializeDeflate()." );
			istate = new InflateManager ( expectRfc1950Header );
			return istate.Initialize ( this, windowBits );
		}

		public int Inflate ( FlushType flush )
		{
			if ( istate == null )
				throw new CompressionProcessException ( "No Inflate State!" );
			return istate.Inflate ( flush );
		}

		public int EndInflate ()
		{
			if ( istate == null )
				throw new CompressionProcessException ( "No Inflate State!" );
			int ret = istate.End ();
			istate = null;
			return ret;
		}

		public int SyncInflate ()
		{
			if ( istate == null )
				throw new CompressionProcessException ( "No Inflate State!" );
			return istate.Sync ();
		}

		public int InitializeDeflate ()
		{
			return _InternalInitializeDeflate ( true );
		}

		public int InitializeDeflate ( CompressionLevel level )
		{
			this.CompressLevel = level;
			return _InternalInitializeDeflate ( true );
		}

		public int InitializeDeflate ( CompressionLevel level, bool wantRfc1950Header )
		{
			this.CompressLevel = level;
			return _InternalInitializeDeflate ( wantRfc1950Header );
		}

		public int InitializeDeflate ( CompressionLevel level, int bits )
		{
			this.CompressLevel = level;
			this.WindowBits = bits;
			return _InternalInitializeDeflate ( true );
		}

		public int InitializeDeflate ( CompressionLevel level, int bits, bool wantRfc1950Header )
		{
			this.CompressLevel = level;
			this.WindowBits = bits;
			return _InternalInitializeDeflate ( wantRfc1950Header );
		}

		private int _InternalInitializeDeflate ( bool wantRfc1950Header )
		{
			if ( istate != null ) throw new CompressionProcessException ( "You may not call InitializeDeflate() after calling InitializeInflate()." );
			dstate = new DeflateManager ();
			dstate.WantRfc1950HeaderBytes = wantRfc1950Header;

			return dstate.Initialize ( this, this.CompressLevel, this.WindowBits, this.Strategy );
		}

		public int Deflate ( FlushType flush )
		{
			if ( dstate == null )
				throw new CompressionProcessException ( "No Deflate State!" );
			return dstate.Deflate ( flush );
		}

		public int EndDeflate ()
		{
			if ( dstate == null )
				throw new CompressionProcessException ( "No Deflate State!" );
			dstate = null;
			return ZlibConstants.Z_OK;
		}

		public void ResetDeflate ()
		{
			if ( dstate == null )
				throw new CompressionProcessException ( "No Deflate State!" );
			dstate.Reset ();
		}

		public int SetDeflateParams ( CompressionLevel level, CompressionStrategy strategy )
		{
			if ( dstate == null )
				throw new CompressionProcessException ( "No Deflate State!" );
			return dstate.SetParams ( level, strategy );
		}

		public int SetDictionary ( byte [] dictionary )
		{
			if ( istate != null )
				return istate.SetDictionary ( dictionary );

			if ( dstate != null )
				return dstate.SetDictionary ( dictionary );

			throw new CompressionProcessException ( "No Inflate or Deflate state!" );
		}

		internal void flush_pending ()
		{
			int len = dstate.pendingCount;

			if ( len > AvailableBytesOut )
				len = AvailableBytesOut;
			if ( len == 0 )
				return;

			if ( dstate.pending.Length <= dstate.nextPending ||
				OutputBuffer.Length <= NextOut ||
				dstate.pending.Length < ( dstate.nextPending + len ) ||
				OutputBuffer.Length < ( NextOut + len ) )
			{
				throw new CompressionProcessException ( String.Format ( "Invalid State. (pending.Length={0}, pendingCount={1})",
					dstate.pending.Length, dstate.pendingCount ) );
			}

			Array.Copy ( dstate.pending, dstate.nextPending, OutputBuffer, NextOut, len );

			NextOut += len;
			dstate.nextPending += len;
			TotalBytesOut += len;
			AvailableBytesOut -= len;
			dstate.pendingCount -= len;
			if ( dstate.pendingCount == 0 )
			{
				dstate.nextPending = 0;
			}
		}

		internal int read_buf ( byte [] buf, int start, int size )
		{
			int len = AvailableBytesIn;

			if ( len > size )
				len = size;
			if ( len == 0 )
				return 0;

			AvailableBytesIn -= len;

			if ( dstate.WantRfc1950HeaderBytes )
			{
				adler32 = Adler.Adler32 ( adler32, InputBuffer, NextIn, len );
			}
			Array.Copy ( InputBuffer, NextIn, buf, start, len );
			NextIn += len;
			TotalBytesIn += len;
			return len;
		}
	}
}