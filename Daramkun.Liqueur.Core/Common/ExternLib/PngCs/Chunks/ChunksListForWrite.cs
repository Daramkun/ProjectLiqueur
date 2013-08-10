namespace Hjg.Pngcs.Chunks
{
	using Hjg.Pngcs;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;

	internal class ChunksListForWrite : ChunksList
	{
		private List<PngChunk> queuedChunks;

		private Dictionary<String, int> alreadyWrittenKeys;

		internal ChunksListForWrite ( ImageInfo info )
			: base ( info )
		{
			this.queuedChunks = new List<PngChunk> ();
			this.alreadyWrittenKeys = new Dictionary<String, int> ();
		}

		public List<PngChunk> GetQueuedById ( String id )
		{
			return GetQueuedById ( id, null );
		}

		public List<PngChunk> GetQueuedById ( String id, String innerid )
		{
			return GetXById ( queuedChunks, id, innerid );
		}

		public PngChunk GetQueuedById1 ( String id, String innerid, bool failIfMultiple )
		{
			List<PngChunk> list = GetQueuedById ( id, innerid );
			if ( list.Count == 0 )
				return null;
			if ( list.Count > 1 && ( failIfMultiple || !list [ 0 ].AllowsMultiple () ) )
				throw new PngjException ( "unexpected multiple chunks id=" + id );
			return list [ list.Count - 1 ];
		}

		public PngChunk GetQueuedById1 ( String id, bool failIfMultiple )
		{
			return GetQueuedById1 ( id, null, failIfMultiple );
		}

		public PngChunk GetQueuedById1 ( String id )
		{
			return GetQueuedById1 ( id, false );
		}

		public bool RemoveChunk ( PngChunk c )
		{
			return queuedChunks.Remove ( c );
		}

		public bool Queue ( PngChunk chunk )
		{
			queuedChunks.Add ( chunk );
			return true;
		}

		private static bool shouldWrite ( PngChunk c, int currentGroup )
		{
			if ( currentGroup == CHUNK_GROUP_2_PLTE )
				return c.Id.Equals ( ChunkHelper.PLTE );
			if ( currentGroup % 2 == 0 )
				throw new PngjOutputException ( "bad chunk group?" );
			int minChunkGroup, maxChunkGroup;
			if ( c.mustGoBeforePLTE () )
				minChunkGroup = maxChunkGroup = ChunksList.CHUNK_GROUP_1_AFTERIDHR;
			else if ( c.mustGoBeforeIDAT () )
			{
				maxChunkGroup = ChunksList.CHUNK_GROUP_3_AFTERPLTE;
				minChunkGroup = c.mustGoAfterPLTE () ? ChunksList.CHUNK_GROUP_3_AFTERPLTE
						: ChunksList.CHUNK_GROUP_1_AFTERIDHR;
			}
			else
			{
				maxChunkGroup = ChunksList.CHUNK_GROUP_5_AFTERIDAT;
				minChunkGroup = ChunksList.CHUNK_GROUP_1_AFTERIDHR;
			}

			int preferred = maxChunkGroup;
			if ( c.Priority )
				preferred = minChunkGroup;
			if ( ChunkHelper.IsUnknown ( c ) && c.ChunkGroup > 0 )
				preferred = c.ChunkGroup;
			if ( currentGroup == preferred )
				return true;
			if ( currentGroup > preferred && currentGroup <= maxChunkGroup )
				return true;
			return false;
		}

		internal int writeChunks ( Stream os, int currentGroup )
		{
			List<int> written = new List<int> ();
			for ( int i = 0; i < queuedChunks.Count; i++ )
			{
				PngChunk c = queuedChunks [ i ];
				if ( !shouldWrite ( c, currentGroup ) )
					continue;
				if ( ChunkHelper.IsCritical ( c.Id ) && !c.Id.Equals ( ChunkHelper.PLTE ) )
					throw new PngjOutputException ( "bad chunk queued: " + c );
				if ( alreadyWrittenKeys.ContainsKey ( c.Id ) && !c.AllowsMultiple () )
					throw new PngjOutputException ( "duplicated chunk does not allow multiple: " + c );
				c.write ( os );
				chunks.Add ( c );
				alreadyWrittenKeys [ c.Id ] = alreadyWrittenKeys.ContainsKey ( c.Id ) ? alreadyWrittenKeys [ c.Id ] + 1 : 1;
				written.Add ( i );
				c.ChunkGroup = currentGroup;
			}
			for ( int k = written.Count - 1; k >= 0; k-- )
			{
				queuedChunks.RemoveAt ( written [ k ] );
			}
			return written.Count;
		}

		internal List<PngChunk> GetQueuedChunks ()
		{
			return queuedChunks;
		}
	}
}
