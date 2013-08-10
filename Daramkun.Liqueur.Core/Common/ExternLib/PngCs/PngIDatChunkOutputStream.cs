namespace Hjg.Pngcs
{
	using Hjg.Pngcs.Chunks;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;

	internal class PngIDatChunkOutputStream : ProgressiveOutputStream
	{
		private const int SIZE_DEFAULT = 32768;
		private readonly Stream outputStream;

		public PngIDatChunkOutputStream ( Stream outputStream_0 )
			: this ( outputStream_0, SIZE_DEFAULT )
		{

		}

		public PngIDatChunkOutputStream ( Stream outputStream_0, int size )
			: base ( size > 8 ? size : SIZE_DEFAULT )
		{
			this.outputStream = outputStream_0;
		}

		protected override void FlushBuffer ( byte [] b, int len )
		{
			ChunkRaw c = new ChunkRaw ( len, Hjg.Pngcs.Chunks.ChunkHelper.b_IDAT, false );
			c.Data = b;
			c.WriteChunk ( outputStream );
		}

		protected override void Dispose ( bool isDisposing )
		{
			Flush ();
		}
	}
}
