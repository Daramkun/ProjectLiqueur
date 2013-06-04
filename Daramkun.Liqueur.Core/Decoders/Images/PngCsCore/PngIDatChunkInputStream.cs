namespace Hjg.Pngcs
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using Hjg.Pngcs.Zlib;

	internal class PngIDatChunkInputStream : Stream
	{
		private readonly Stream inputStream;
		private readonly Hjg.Pngcs.Zlib.CRC32 crcEngine;
		private bool checkCrc;
		private int lenLastChunk;
		private byte [] idLastChunk;
		private int toReadThisChunk;
		private bool ended;
		private long offset;

		public class IdatChunkInfo
		{
			public readonly int len;
			public readonly long offset;
			public IdatChunkInfo ( int len_0, long offset_1 )
			{
				this.len = len_0;
				this.offset = offset_1;
			}
		}

		public override void Write ( byte [] buffer, int offset, int count ) { }
		public override void SetLength ( long value ) { }
		public override long Seek ( long offset, SeekOrigin origin ) { return -1; }
		public override void Flush () { }
		public override long Position { get; set; }
		public override long Length { get { return 0; } }
		public override bool CanWrite { get { return false; } }
		public override bool CanRead { get { return true; } }
		public override bool CanSeek { get { return false; } }

		public IList<IdatChunkInfo> foundChunksInfo;

		public PngIDatChunkInputStream ( Stream iStream, int lenFirstChunk, long offset_0 )
		{
			this.idLastChunk = new byte [ 4 ];
			this.toReadThisChunk = 0;
			this.ended = false;
			this.foundChunksInfo = new List<IdatChunkInfo> ();
			this.offset = offset_0;
			checkCrc = true;
			inputStream = iStream;
			crcEngine = new CRC32 ();
			this.lenLastChunk = lenFirstChunk;
			toReadThisChunk = lenFirstChunk;
			System.Array.Copy ( ( Array ) ( Hjg.Pngcs.Chunks.ChunkHelper.b_IDAT ), 0, ( Array ) ( idLastChunk ), 0, 4 );
			crcEngine.Update ( idLastChunk, 0, 4 );
			foundChunksInfo.Add ( new PngIDatChunkInputStream.IdatChunkInfo ( lenLastChunk, offset_0 - 8 ) );
			if ( this.lenLastChunk == 0 )
				EndChunkGoForNext ();
		}

		protected override void Dispose ( bool isDisposing )
		{
			base.Dispose ( isDisposing );
		}

		private void EndChunkGoForNext ()
		{
			do
			{
				int crc = Hjg.Pngcs.PngHelperInternal.ReadInt4 ( inputStream ); //
				offset += 4;
				if ( checkCrc )
				{
					int crccalc = ( int ) crcEngine.GetValue ();
					if ( lenLastChunk > 0 && crc != crccalc )
						throw new PngjBadCrcException ( "error reading idat; offset: " + offset );
					crcEngine.Reset ();
				}
				lenLastChunk = Hjg.Pngcs.PngHelperInternal.ReadInt4 ( inputStream );
				if ( lenLastChunk < 0 )
					throw new PngjInputException ( "invalid len for chunk: " + lenLastChunk );
				toReadThisChunk = lenLastChunk;
				Hjg.Pngcs.PngHelperInternal.ReadBytes ( inputStream, idLastChunk, 0, 4 );
				offset += 8;

				ended = !PngCsUtils.arraysEqual4 ( idLastChunk, Hjg.Pngcs.Chunks.ChunkHelper.b_IDAT );
				if ( !ended )
				{
					foundChunksInfo.Add ( new PngIDatChunkInputStream.IdatChunkInfo ( lenLastChunk, ( offset - 8 ) ) );
					if ( checkCrc )
						crcEngine.Update ( idLastChunk, 0, 4 );
				}
			} while ( lenLastChunk == 0 && !ended );
		}

		public void ForceChunkEnd ()
		{
			if ( !ended )
			{
				byte [] dummy = new byte [ toReadThisChunk ];
				Hjg.Pngcs.PngHelperInternal.ReadBytes ( inputStream, dummy, 0, toReadThisChunk );
				if ( checkCrc )
					crcEngine.Update ( dummy, 0, toReadThisChunk );
				EndChunkGoForNext ();
			}
		}

		public override int Read ( byte [] b, int off, int len_0 )
		{
			if ( ended )
				return -1;
			if ( toReadThisChunk == 0 ) throw new Exception ( "this should not happen" );
			int n = inputStream.Read ( b, off, ( len_0 >= toReadThisChunk ) ? toReadThisChunk : len_0 );
			if ( n == -1 ) n = -2;
			if ( n > 0 )
			{
				if ( checkCrc )
					crcEngine.Update ( b, off, n );
				this.offset += n;
				toReadThisChunk -= n;
			}
			if ( n >= 0 && toReadThisChunk == 0 )
			{
				EndChunkGoForNext ();
			}
			return n;
		}

		public int Read ( byte [] b )
		{
			return this.Read ( b, 0, b.Length );
		}

		public override int ReadByte ()
		{
			byte [] b1 = new byte [ 1 ];
			int r = this.Read ( b1, 0, 1 );
			return ( r < 0 ) ? -1 : ( int ) b1 [ 0 ];
		}

		public int GetLenLastChunk ()
		{
			return lenLastChunk;
		}

		public byte [] GetIdLastChunk ()
		{
			return idLastChunk;
		}

		public long GetOffset ()
		{
			return offset;
		}

		public bool IsEnded ()
		{
			return ended;
		}

		internal void DisableCrcCheck ()
		{
			checkCrc = false;
		}
	}
}
