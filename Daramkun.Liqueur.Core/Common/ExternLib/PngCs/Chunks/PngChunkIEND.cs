namespace Hjg.Pngcs.Chunks
{
	using Hjg.Pngcs;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;

	internal class PngChunkIEND : PngChunkSingle
	{
		public const String ID = ChunkHelper.IEND;

		public PngChunkIEND ( ImageInfo info )
			: base ( ID, info )
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint ()
		{
			return ChunkOrderingConstraint.NA;
		}

		public override ChunkRaw CreateRawChunk ()
		{
			ChunkRaw c = new ChunkRaw ( 0, ChunkHelper.b_IEND, false );
			return c;
		}

		public override void ParseFromRaw ( ChunkRaw c )
		{
		}

		public override void CloneDataFromRead ( PngChunk other )
		{
		}
	}
}
