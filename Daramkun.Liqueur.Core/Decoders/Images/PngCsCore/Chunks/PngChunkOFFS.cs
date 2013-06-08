namespace Hjg.Pngcs.Chunks
{
	using Hjg.Pngcs;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;

	internal class PngChunkOFFS : PngChunkSingle
	{
		public const String ID = "oFFs";

		private long posX;
		private long posY;
		private int units;

		public PngChunkOFFS ( ImageInfo info )
			: base ( ID, info ) { }

		public override ChunkOrderingConstraint GetOrderingConstraint ()
		{
			return ChunkOrderingConstraint.BEFORE_IDAT;
		}

		public override ChunkRaw CreateRawChunk ()
		{
			ChunkRaw c = createEmptyChunk ( 9, true );
			PngHelperInternal.WriteInt4tobytes ( ( int ) posX, c.Data, 0 );
			PngHelperInternal.WriteInt4tobytes ( ( int ) posY, c.Data, 4 );
			c.Data [ 8 ] = ( byte ) units;
			return c;
		}

		public override void ParseFromRaw ( ChunkRaw chunk )
		{
			if ( chunk.Length != 9 )
				throw new PngjException ( "bad chunk length " + chunk );
			posX = PngHelperInternal.ReadInt4fromBytes ( chunk.Data, 0 );
			if ( posX < 0 )
				posX += 0x100000000L;
			posY = PngHelperInternal.ReadInt4fromBytes ( chunk.Data, 4 );
			if ( posY < 0 )
				posY += 0x100000000L;
			units = PngHelperInternal.ReadInt1fromByte ( chunk.Data, 8 );
		}

		public override void CloneDataFromRead ( PngChunk other )
		{
			PngChunkOFFS otherx = ( PngChunkOFFS ) other;
			this.posX = otherx.posX;
			this.posY = otherx.posY;
			this.units = otherx.units;
		}

		public int GetUnits ()
		{
			return units;
		}

		public void SetUnits ( int units )
		{
			this.units = units;
		}

		public long GetPosX ()
		{
			return posX;
		}

		public void SetPosX ( long posX )
		{
			this.posX = posX;
		}

		public long GetPosY ()
		{
			return posY;
		}

		public void SetPosY ( long posY )
		{
			this.posY = posY;
		}
	}
}
