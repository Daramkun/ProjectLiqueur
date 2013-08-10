namespace Hjg.Pngcs.Chunks
{
	using Hjg.Pngcs;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;

	internal class PngChunkUNKNOWN : PngChunkMultiple
	{
		private byte [] data;

		public PngChunkUNKNOWN ( String id, ImageInfo info )
			: base ( id, info )
		{
		}

		private PngChunkUNKNOWN ( PngChunkUNKNOWN c, ImageInfo info )
			: base ( c.Id, info )
		{
			System.Array.Copy ( c.data, 0, data, 0, c.data.Length );
		}

		public override ChunkOrderingConstraint GetOrderingConstraint ()
		{
			return ChunkOrderingConstraint.NONE;
		}

		public override ChunkRaw CreateRawChunk ()
		{
			ChunkRaw p = createEmptyChunk ( data.Length, false );
			p.Data = this.data;
			return p;
		}

		public override void ParseFromRaw ( ChunkRaw c )
		{
			data = c.Data;
		}

		public byte [] GetData ()
		{
			return data;
		}

		public void SetData ( byte [] data_0 )
		{
			this.data = data_0;
		}

		public override void CloneDataFromRead ( PngChunk other )
		{
			PngChunkUNKNOWN c = ( PngChunkUNKNOWN ) other;
			data = c.data;
		}
	}
}
