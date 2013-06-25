using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Daramkun.Liqueur.IO.Compression;

namespace Hjg.Pngcs.Zlib
{
	class ZlibInputStreamMs : AZlibInputStream
	{
		public ZlibInputStreamMs ( Stream st, bool leaveOpen )
			: base ( st, leaveOpen )
		{
		}

		private DeflateStream deflateStream;
		private bool initdone = false;
		private bool closed = false;

		private bool fdict;
		private int cmdinfo;
		private byte [] dictid;
		private byte [] crcread = null;

		public override int Read ( byte [] array, int offset, int count )
		{
			if ( !initdone ) doInit ();
			if ( deflateStream == null && count > 0 ) initStream ();
			int r = deflateStream.Read ( array, offset, count );
			if ( r < 1 && crcread == null )
			{
				crcread = new byte [ 4 ];
				for ( int i = 0; i < 4; i++ ) crcread [ i ] = ( byte ) rawStream.ReadByte ();
			}
			return r;
		}

		protected override void Dispose ( bool isDisposing )
		{
			if ( !initdone ) doInit ();
			if ( closed ) return;
			closed = true;
			if ( deflateStream != null )
			{
				deflateStream.Dispose ();
			}
			if ( crcread == null )
			{
				crcread = new byte [ 4 ];
				for ( int i = 0; i < 4; i++ ) crcread [ i ] = ( byte ) rawStream.ReadByte ();
			}
			if ( !leaveOpen )
				rawStream.Dispose ();
		}

		private void initStream ()
		{
			if ( deflateStream != null ) return;
			deflateStream = new DeflateStream ( rawStream, CompressionMode.Decompress, true );
		}

		private void doInit ()
		{
			if ( initdone ) return;
			initdone = true;

			int cmf = rawStream.ReadByte ();
			int flag = rawStream.ReadByte ();
			if ( cmf == -1 || flag == -1 ) return;
			if ( ( cmf & 0x0f ) != 8 ) throw new Exception ( "Bad compression method for ZLIB header: cmf=" + cmf );
			cmdinfo = ( ( cmf & ( 0xf0 ) ) >> 8 );
			fdict = ( flag & 32 ) != 0;
			if ( fdict )
			{
				dictid = new byte [ 4 ];
				for ( int i = 0; i < 4; i++ )
				{
					dictid [ i ] = ( byte ) rawStream.ReadByte ();
				}
			}
		}

		public override void Flush ()
		{
			if ( deflateStream != null ) deflateStream.Flush ();
		}

		public override String getImplementationId ()
		{
			return "Zlib inflater: .Net CLR 4.5";
		}
	}
}
