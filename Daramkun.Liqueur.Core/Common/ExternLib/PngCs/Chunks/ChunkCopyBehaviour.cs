using System;
using System.Collections.Generic;
using System.Text;

namespace Hjg.Pngcs.Chunks
{
	internal class ChunkCopyBehaviour
	{
		public static readonly int COPY_NONE = 0;
		public static readonly int COPY_PALETTE = 1;
		public static readonly int COPY_ALL_SAFE = 1 << 2;
		public static readonly int COPY_ALL = 1 << 3;
		public static readonly int COPY_PHYS = 1 << 4;
		public static readonly int COPY_TEXTUAL = 1 << 5;
		public static readonly int COPY_TRANSPARENCY = 1 << 6;
		public static readonly int COPY_UNKNOWN = 1 << 7;
		public static readonly int COPY_ALMOSTALL = 1 << 8;
	}
}
