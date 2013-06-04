using System;
using System.Collections.Generic;
using System.Text;

namespace Hjg.Pngcs.Chunks
{
	internal interface ChunkPredicate
	{
		bool Matches ( PngChunk chunk );
	}
}
