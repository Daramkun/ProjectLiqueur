namespace Hjg.Pngcs.Chunks
{
	using Hjg.Pngcs;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;

	internal class PngChunkIDAT : PngChunkMultiple
	{
		public const String ID = ChunkHelper.IDAT;

		public PngChunkIDAT ( ImageInfo i, int len, long offset )
			: base ( ID, i )
		{
			this.Length = len;
			this.Offset = offset;
		}

		public override ChunkOrderingConstraint GetOrderingConstraint ()
		{
			return ChunkOrderingConstraint.NA;
		}

		public override ChunkRaw CreateRawChunk ()
		{
			return null;
		}

		public override void ParseFromRaw ( ChunkRaw c )
		{
		}

		public override void CloneDataFromRead ( PngChunk other )
		{
		}
	}
}
