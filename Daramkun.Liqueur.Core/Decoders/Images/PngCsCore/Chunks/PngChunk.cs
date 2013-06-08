namespace Hjg.Pngcs.Chunks
{
	using Hjg.Pngcs;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Diagnostics;

	internal abstract class PngChunk
	{
		public readonly String Id;
		public readonly bool Crit, Pub, Safe;

		protected readonly ImageInfo ImgInfo;

		public bool Priority { get; set; }
		public int ChunkGroup { get; set; }

		public int Length { get; set; }
		public long Offset { get; set; }

		public enum ChunkOrderingConstraint
		{
			NONE,
			BEFORE_PLTE_AND_IDAT,
			AFTER_PLTE_BEFORE_IDAT,
			BEFORE_IDAT,
			NA
		}

		protected PngChunk ( String id, ImageInfo imgInfo )
		{
			this.Id = id;
			this.ImgInfo = imgInfo;
			this.Crit = Hjg.Pngcs.Chunks.ChunkHelper.IsCritical ( id );
			this.Pub = Hjg.Pngcs.Chunks.ChunkHelper.IsPublic ( id );
			this.Safe = Hjg.Pngcs.Chunks.ChunkHelper.IsSafeToCopy ( id );
			this.Priority = false;
			this.ChunkGroup = -1;
			this.Length = -1;
			this.Offset = 0;
		}

		private static Dictionary<String, Type> factoryMap = initFactory ();

		private static Dictionary<String, Type> initFactory ()
		{
			Dictionary<String, Type> f = new Dictionary<string, System.Type> ();
			f.Add ( ChunkHelper.IDAT, typeof ( PngChunkIDAT ) );
			f.Add ( ChunkHelper.IHDR, typeof ( PngChunkIHDR ) );
			f.Add ( ChunkHelper.PLTE, typeof ( PngChunkPLTE ) );
			f.Add ( ChunkHelper.IEND, typeof ( PngChunkIEND ) );
			f.Add ( ChunkHelper.tEXt, typeof ( PngChunkTEXT ) );
			f.Add ( ChunkHelper.iTXt, typeof ( PngChunkITXT ) );
			f.Add ( ChunkHelper.zTXt, typeof ( PngChunkZTXT ) );
			f.Add ( ChunkHelper.bKGD, typeof ( PngChunkBKGD ) );
			f.Add ( ChunkHelper.gAMA, typeof ( PngChunkGAMA ) );
			f.Add ( ChunkHelper.pHYs, typeof ( PngChunkPHYS ) );
			f.Add ( ChunkHelper.iCCP, typeof ( PngChunkICCP ) );
			f.Add ( ChunkHelper.tIME, typeof ( PngChunkTIME ) );
			f.Add ( ChunkHelper.tRNS, typeof ( PngChunkTRNS ) );
			f.Add ( ChunkHelper.cHRM, typeof ( PngChunkCHRM ) );
			f.Add ( ChunkHelper.sBIT, typeof ( PngChunkSBIT ) );
			f.Add ( ChunkHelper.sRGB, typeof ( PngChunkSRGB ) );
			f.Add ( ChunkHelper.hIST, typeof ( PngChunkHIST ) );
			f.Add ( ChunkHelper.sPLT, typeof ( PngChunkSPLT ) );

			f.Add ( PngChunkOFFS.ID, typeof ( PngChunkOFFS ) );
			f.Add ( PngChunkSTER.ID, typeof ( PngChunkSTER ) );
			return f;
		}

		public static void FactoryRegister ( String chunkId, Type type )
		{
			factoryMap.Add ( chunkId, type );
		}

		internal static bool isKnown ( String id )
		{
			return factoryMap.ContainsKey ( id );
		}

		internal bool mustGoBeforePLTE ()
		{
			return GetOrderingConstraint () == ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
		}

		internal bool mustGoBeforeIDAT ()
		{
			ChunkOrderingConstraint oc = GetOrderingConstraint ();
			return oc == ChunkOrderingConstraint.BEFORE_IDAT || oc == ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT || oc == ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
		}

		internal bool mustGoAfterPLTE ()
		{
			return GetOrderingConstraint () == ChunkOrderingConstraint.AFTER_PLTE_BEFORE_IDAT;
		}

		internal static PngChunk Factory ( ChunkRaw chunk, ImageInfo info )
		{
			PngChunk c = FactoryFromId ( Hjg.Pngcs.Chunks.ChunkHelper.ToString ( chunk.IdBytes ), info );
			c.Length = chunk.Length;
			c.ParseFromRaw ( chunk );
			return c;
		}

		internal static PngChunk FactoryFromId ( String cid, ImageInfo info )
		{
			PngChunk chunk = null;
			if ( factoryMap == null ) initFactory ();
			if ( isKnown ( cid ) )
			{
				Type t = factoryMap [ cid ];
				if ( t == null ) Debug.WriteLine ( "What?? " + cid );
				object o = Activator.CreateInstance ( t, info );
				chunk = ( PngChunk ) o;
			}
			if ( chunk == null )
				chunk = new PngChunkUNKNOWN ( cid, info );

			return chunk;
		}

		public ChunkRaw createEmptyChunk ( int len, bool alloc )
		{
			ChunkRaw c = new ChunkRaw ( len, ChunkHelper.ToBytes ( Id ), alloc );
			return c;
		}

		public static T CloneChunk<T> ( T chunk, ImageInfo info ) where T : PngChunk
		{
			PngChunk cn = FactoryFromId ( chunk.Id, info );
			if ( ( Object ) cn.GetType () != ( Object ) chunk.GetType () )
				throw new PngjException ( "bad class cloning chunk: " + cn.GetType () + " "
						+ chunk.GetType () );
			cn.CloneDataFromRead ( chunk );
			return ( T ) cn;
		}

		internal void write ( Stream os )
		{
			ChunkRaw c = CreateRawChunk ();
			if ( c == null )
				throw new PngjException ( "null chunk ! creation failed for " + this );
			c.WriteChunk ( os );
		}

		public override String ToString ()
		{
			return "chunk id= " + Id + " (len=" + Length + " off=" + Offset + ") c=" + GetType ().Name;
		}

		public abstract ChunkRaw CreateRawChunk ();
		public abstract void ParseFromRaw ( ChunkRaw c );
		public abstract void CloneDataFromRead ( PngChunk other );
		public abstract bool AllowsMultiple ();
		public abstract ChunkOrderingConstraint GetOrderingConstraint ();
	}
}
