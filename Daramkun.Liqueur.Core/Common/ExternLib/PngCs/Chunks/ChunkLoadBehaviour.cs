namespace Hjg.Pngcs.Chunks
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;

	internal enum ChunkLoadBehaviour
	{
		LOAD_CHUNK_NEVER,
		LOAD_CHUNK_KNOWN,
		LOAD_CHUNK_IF_SAFE,
		LOAD_CHUNK_ALWAYS,
	}
}
