namespace Hjg.Pngcs.Chunks
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Text;

	internal class ChunksList
	{
		internal const int CHUNK_GROUP_0_IDHR = 0;
		internal const int CHUNK_GROUP_1_AFTERIDHR = 1;
		internal const int CHUNK_GROUP_2_PLTE = 2;
		internal const int CHUNK_GROUP_3_AFTERPLTE = 3;
		internal const int CHUNK_GROUP_4_IDAT = 4;
		internal const int CHUNK_GROUP_5_AFTERIDAT = 5;
		internal const int CHUNK_GROUP_6_END = 6;

		protected List<PngChunk> chunks;

		internal readonly ImageInfo imageInfo;

		internal ChunksList ( ImageInfo imfinfo )
		{
			this.chunks = new List<PngChunk> ();
			this.imageInfo = imfinfo;
		}

		public Dictionary<String, int> GetChunksKeys ()
		{
			Dictionary<String, int> ck = new Dictionary<String, int> ();
			foreach ( PngChunk c in chunks )
			{
				ck [ c.Id ] = ck.ContainsKey ( c.Id ) ? ck [ c.Id ] + 1 : 1;
			}
			return ck;
		}

		public List<PngChunk> GetChunks ()
		{
			return new List<PngChunk> ( chunks );
		}

		internal static List<PngChunk> GetXById ( List<PngChunk> list, String id, String innerid )
		{
			if ( innerid == null )
				return ChunkHelper.FilterList ( list, new ChunkPredicateId ( id ) );
			else
				return ChunkHelper.FilterList ( list, new ChunkPredicateId2 ( id, innerid ) );
		}

		public void AppendReadChunk ( PngChunk chunk, int chunkGroup )
		{
			chunk.ChunkGroup = chunkGroup;
			chunks.Add ( chunk );
		}

		public List<PngChunk> GetById ( String id )
		{
			return GetById ( id, null );
		}

		public List<PngChunk> GetById ( String id, String innerid )
		{
			return GetXById ( chunks, id, innerid );
		}

		public PngChunk GetById1 ( String id )
		{
			return GetById1 ( id, false );
		}

		public PngChunk GetById1 ( String id, bool failIfMultiple )
		{
			return GetById1 ( id, null, failIfMultiple );
		}

		public PngChunk GetById1 ( String id, String innerid, bool failIfMultiple )
		{
			List<PngChunk> list = GetById ( id, innerid );
			if ( list.Count == 0 )
				return null;
			if ( list.Count > 1 && ( failIfMultiple || !list [ 0 ].AllowsMultiple () ) )
				throw new PngjException ( "unexpected multiple chunks id=" + id );
			return list [ list.Count - 1 ];
		}

		public List<PngChunk> GetEquivalent ( PngChunk chunk )
		{
			return ChunkHelper.FilterList ( chunks, new ChunkPredicateEquiv ( chunk ) );
		}

		public override String ToString ()
		{
			return "ChunkList: read: " + chunks.Count;
		}

		public String ToStringFull ()
		{
			StringBuilder sb = new StringBuilder ( ToString () );
			sb.Append ( "\n Read:\n" );
			foreach ( PngChunk chunk in chunks )
			{
				sb.Append ( chunk ).Append ( " G=" + chunk.ChunkGroup + "\n" );
			}
			return sb.ToString ();
		}
	}
}
