using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Daramkun.Liqueur.IO.Compression;

namespace Hjg.Pngcs.Zlib
{
	class ZlibOutputStreamMs : AZlibOutputStream
	{
		public ZlibOutputStreamMs ( Stream st, int compressLevel, EDeflateCompressStrategy strat, bool leaveOpen )
			: base ( st, compressLevel, strat, leaveOpen )
		{
		}

		private DeflateStream deflateStream;
		private Adler32 adler32 = new Adler32 ();
		private bool initdone = false;
		private bool closed = false;

		public override void WriteByte ( byte value )
		{
			if ( !initdone ) doInit ();
			if ( deflateStream == null ) initStream ();
			base.WriteByte ( value );
			adler32.Update ( value );
		}

		public override void Write ( byte [] array, int offset, int count )
		{
			if ( count == 0 ) return;
			if ( !initdone ) doInit ();
			if ( deflateStream == null ) initStream ();
			deflateStream.Write ( array, offset, count );
			adler32.Update ( array, offset, count );
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
			else
			{
				rawStream.WriteByte ( 3 );
				rawStream.WriteByte ( 0 );
			}

			uint crcv = adler32.GetValue ();
			rawStream.WriteByte ( ( byte ) ( ( crcv >> 24 ) & 0xFF ) );
			rawStream.WriteByte ( ( byte ) ( ( crcv >> 16 ) & 0xFF ) );
			rawStream.WriteByte ( ( byte ) ( ( crcv >> 8 ) & 0xFF ) );
			rawStream.WriteByte ( ( byte ) ( ( crcv ) & 0xFF ) );
			if ( !leaveOpen )
				rawStream.Dispose ();

		}

		private void initStream ()
		{
			if ( deflateStream != null ) return;
			CompressionLevel clevel = CompressionLevel.Level9;
			
			if ( compressLevel >= 1 && compressLevel <= 5 ) clevel = CompressionLevel.Level1;
			else if ( compressLevel == 0 ) clevel = CompressionLevel.None;
			deflateStream = new DeflateStream ( rawStream, CompressionMode.Compress, clevel, true );
		}

		private void doInit ()
		{
			if ( initdone ) return;
			initdone = true;

			int cmf = 0x78;
			int flg = 218;
			if ( compressLevel >= 5 && compressLevel <= 6 ) flg = 156;
			else if ( compressLevel >= 3 && compressLevel <= 4 ) flg = 94;
			else if ( compressLevel <= 2 ) flg = 1;
			flg -= ( ( cmf * 256 + flg ) % 31 );
			if ( flg < 0 ) flg += 31;
			rawStream.WriteByte ( ( byte ) cmf );
			rawStream.WriteByte ( ( byte ) flg );

		}

		public override void Flush ()
		{
			if ( deflateStream != null ) deflateStream.Flush ();
		}

		public override String getImplementationId ()
		{
			return "Zlib deflater: .Net CLR 4.5";
		}

	}
}
