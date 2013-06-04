using System;
using System.Collections.Generic;
using System.Text;

namespace Hjg.Pngcs.Chunks
{
	internal abstract class PngChunkMultiple : PngChunk
	{
		internal PngChunkMultiple ( String id, ImageInfo imgInfo )
			: base ( id, imgInfo )
		{

		}

		public sealed override bool AllowsMultiple ()
		{
			return true;
		}

	}
}
