namespace Hjg.Pngcs.Chunks
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using Hjg.Pngcs.Zlib;

	internal class ChunkHelper
	{
		internal const String IHDR = "IHDR";
		internal const String PLTE = "PLTE";
		internal const String IDAT = "IDAT";
		internal const String IEND = "IEND";
		internal const String cHRM = "cHRM";
		internal const String gAMA = "gAMA";
		internal const String iCCP = "iCCP";
		internal const String sBIT = "sBIT";
		internal const String sRGB = "sRGB";
		internal const String bKGD = "bKGD";
		internal const String hIST = "hIST";
		internal const String tRNS = "tRNS";
		internal const String pHYs = "pHYs";
		internal const String sPLT = "sPLT";
		internal const String tIME = "tIME";
		internal const String iTXt = "iTXt";
		internal const String tEXt = "tEXt";
		internal const String zTXt = "zTXt";
		internal static readonly byte [] b_IHDR = ToBytes ( IHDR );
		internal static readonly byte [] b_PLTE = ToBytes ( PLTE );
		internal static readonly byte [] b_IDAT = ToBytes ( IDAT );
		internal static readonly byte [] b_IEND = ToBytes ( IEND );

		public static byte [] ToBytes ( String x )
		{
			return Hjg.Pngcs.PngHelperInternal.charsetLatin1.GetBytes ( x );
		}

		public static String ToString ( byte [] x )
		{
			return Hjg.Pngcs.PngHelperInternal.charsetLatin1.GetString ( x, 0, x.Length );
		}

		public static String ToString ( byte [] x, int offset, int len )
		{
			return Hjg.Pngcs.PngHelperInternal.charsetLatin1.GetString ( x, offset, len );
		}

		public static byte [] ToBytesUTF8 ( String x )
		{
			return Hjg.Pngcs.PngHelperInternal.charsetUtf8.GetBytes ( x );
		}

		public static String ToStringUTF8 ( byte [] x )
		{
			return Hjg.Pngcs.PngHelperInternal.charsetUtf8.GetString ( x, 0, x.Length );
		}

		public static String ToStringUTF8 ( byte [] x, int offset, int len )
		{
			return Hjg.Pngcs.PngHelperInternal.charsetUtf8.GetString ( x, offset, len );
		}

		public static void WriteBytesToStream ( Stream stream, byte [] bytes )
		{
			stream.Write ( bytes, 0, bytes.Length );
		}

		public static bool IsCritical ( String id )
		{
			return ( Char.IsUpper ( id [ 0 ] ) );
		}

		public static bool IsPublic ( String id )
		{
			return ( Char.IsUpper ( id [ 1 ] ) );
		}

		public static bool IsSafeToCopy ( String id )
		{
			return ( !Char.IsUpper ( id [ 3 ] ) );
		}

		public static bool IsUnknown ( PngChunk chunk )
		{
			return chunk is PngChunkUNKNOWN;
		}

		public static int PosNullByte ( byte [] bytes )
		{
			for ( int i = 0; i < bytes.Length; i++ )
				if ( bytes [ i ] == 0 )
					return i;
			return -1;
		}

		public static bool ShouldLoad ( String id, ChunkLoadBehaviour behav )
		{
			if ( IsCritical ( id ) )
				return true;
			bool kwown = PngChunk.isKnown ( id );
			switch ( behav )
			{
				case ChunkLoadBehaviour.LOAD_CHUNK_ALWAYS:
					return true;
				case ChunkLoadBehaviour.LOAD_CHUNK_IF_SAFE:
					return kwown || IsSafeToCopy ( id );
				case ChunkLoadBehaviour.LOAD_CHUNK_KNOWN:
					return kwown;
				case ChunkLoadBehaviour.LOAD_CHUNK_NEVER:
					return false;
			}
			return false;
		}

		internal static byte [] compressBytes ( byte [] ori, bool compress )
		{
			return compressBytes ( ori, 0, ori.Length, compress );
		}

		internal static byte [] compressBytes ( byte [] ori, int offset, int len, bool compress )
		{
			try
			{
				MemoryStream inb = new MemoryStream ( ori, offset, len );
				Stream inx = inb;
				if ( !compress ) inx = ZlibStreamFactory.createZlibInputStream ( inb );
				MemoryStream outb = new MemoryStream ();
				Stream outx = outb;
				if ( compress ) outx = ZlibStreamFactory.createZlibOutputStream ( outb );
				shovelInToOut ( inx, outx );
				inx.Dispose ();
				outx.Dispose ();
				byte [] res = outb.ToArray ();
				return res;
			}
			catch ( Exception e )
			{
				throw new PngjException ( e );
			}
		}

		private static void shovelInToOut ( Stream inx, Stream outx )
		{
			byte [] buffer = new byte [ 1024 ];
			int len;
			while ( ( len = inx.Read ( buffer, 0, 1024 ) ) > 0 )
			{
				outx.Write ( buffer, 0, len );
			}
		}

		internal static bool maskMatch ( int v, int mask )
		{
			return ( v & mask ) != 0;
		}

		public static List<PngChunk> FilterList ( List<PngChunk> list, ChunkPredicate predicateKeep )
		{
			List<PngChunk> result = new List<PngChunk> ();
			foreach ( PngChunk element in list )
			{
				if ( predicateKeep.Matches ( element ) )
				{
					result.Add ( element );
				}
			}
			return result;
		}

		public static int TrimList ( List<PngChunk> list, ChunkPredicate predicateRemove )
		{
			int cont = 0;
			for ( int i = list.Count - 1; i >= 0; i-- )
			{
				if ( predicateRemove.Matches ( list [ i ] ) )
				{
					list.RemoveAt ( i );
					cont++;
				}
			}
			return cont;
		}

		public static bool Equivalent ( PngChunk c1, PngChunk c2 )
		{
			if ( c1 == c2 )
				return true;
			if ( c1 == null || c2 == null || !c1.Id.Equals ( c2.Id ) )
				return false;
			if ( c1.GetType () != c2.GetType () )
				return false;
			if ( !c2.AllowsMultiple () )
				return true;
			if ( c1 is PngChunkTextVar )
			{
				return ( ( PngChunkTextVar ) c1 ).GetKey ().Equals ( ( ( PngChunkTextVar ) c2 ).GetKey () );
			}
			if ( c1 is PngChunkSPLT )
			{
				return ( ( PngChunkSPLT ) c1 ).PalName.Equals ( ( ( PngChunkSPLT ) c2 ).PalName );
			}
			return false;
		}

		public static bool IsText ( PngChunk c )
		{
			return c is PngChunkTextVar;
		}

	}
}
