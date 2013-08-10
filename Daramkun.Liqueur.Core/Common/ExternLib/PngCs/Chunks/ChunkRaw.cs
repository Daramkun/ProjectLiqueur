namespace Hjg.Pngcs.Chunks
{
	using Hjg.Pngcs;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;
	using Hjg.Pngcs.Zlib;

	internal class ChunkRaw
	{
		public readonly int Length;
		public readonly byte [] IdBytes;
		public byte [] Data;
		private int crcval;

		internal ChunkRaw ( int length, byte [] idbytes, bool alloc )
		{
			this.IdBytes = new byte [ 4 ];
			this.Data = null;
			this.crcval = 0;
			this.Length = length;
			System.Array.Copy ( ( Array ) ( idbytes ), 0, ( Array ) ( this.IdBytes ), 0, 4 );
			if ( alloc )
				AllocData ();
		}

		private int ComputeCrc ()
		{
			CRC32 crcengine = Hjg.Pngcs.PngHelperInternal.GetCRC ();
			crcengine.Reset ();
			crcengine.Update ( IdBytes, 0, 4 );
			if ( Length > 0 )
				crcengine.Update ( Data, 0, Length ); //
			return ( int ) crcengine.GetValue ();
		}

		internal void WriteChunk ( Stream os )
		{
			if ( IdBytes.Length != 4 )
				throw new PngjOutputException ( "bad chunkid [" + Hjg.Pngcs.Chunks.ChunkHelper.ToString ( IdBytes ) + "]" );
			crcval = ComputeCrc ();
			Hjg.Pngcs.PngHelperInternal.WriteInt4 ( os, Length );
			Hjg.Pngcs.PngHelperInternal.WriteBytes ( os, IdBytes );
			if ( Length > 0 )
				Hjg.Pngcs.PngHelperInternal.WriteBytes ( os, Data, 0, Length );
			Hjg.Pngcs.PngHelperInternal.WriteInt4 ( os, crcval );
		}

		internal int ReadChunkData ( Stream stream, bool checkCrc )
		{
			Hjg.Pngcs.PngHelperInternal.ReadBytes ( stream, Data, 0, Length );
			crcval = Hjg.Pngcs.PngHelperInternal.ReadInt4 ( stream );
			if ( checkCrc )
			{
				int crc = ComputeCrc ();
				if ( crc != crcval )
					throw new PngjBadCrcException ( "crc invalid for chunk " + ToString () + " calc="
							+ crc + " read=" + crcval );
			}
			return Length + 4;
		}

		internal MemoryStream GetAsByteStream ()
		{
			return new MemoryStream ( Data );
		}

		private void AllocData ()
		{
			if ( Data == null || Data.Length < Length )
				Data = new byte [ Length ];
		}
		public override String ToString ()
		{
			return "chunkid=" + Hjg.Pngcs.Chunks.ChunkHelper.ToString ( IdBytes ) + " len=" + Length;
		}
	}
}
