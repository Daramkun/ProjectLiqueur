using System;
using System.Collections.Generic;
using System.Text;

namespace Hjg.Pngcs.Chunks
{
	internal abstract class PngChunkSingle : PngChunk
	{
		public PngChunkSingle ( String id, ImageInfo imgInfo )
			: base ( id, imgInfo )
		{
		}

		public sealed override bool AllowsMultiple ()
		{
			return false;
		}

		public override int GetHashCode ()
		{
			int prime = 31;
			int result = 1;
			result = prime * result + ( ( Id == null ) ? 0 : Id.GetHashCode () );
			return result;
		}

		public override bool Equals ( object obj )
		{
			return ( obj is PngChunkSingle && Id != null && Id.Equals ( ( ( PngChunkSingle ) obj ).Id ) );
		}
	}
}
