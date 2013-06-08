namespace Hjg.Pngcs
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;

	using System.Runtime.CompilerServices;
	using Chunks;
	using Hjg.Pngcs.Zlib;

	internal class PngWriter
	{
		public readonly ImageInfo ImgInfo;

		protected readonly String filename;

		private FilterWriteStrategy filterStrat;

		public EDeflateCompressStrategy CompressionStrategy { get; set; }

		public int CompLevel { get; set; }
		public bool ShouldCloseStream { get; set; }
		public int IdatMaxSize { get; set; }

		private readonly PngMetadata metadata;
		private readonly ChunksListForWrite chunksList;

		protected byte [] rowb;
		protected byte [] rowbprev;
		protected byte [] rowbfilter;

		public int CurrentChunkGroup { get; private set; }

		private int rowNum = -1;
		private readonly Stream outputStream;

		private PngIDatChunkOutputStream datStream;
		private AZlibOutputStream datStreamDeflated;

		private int [] histox = new int [ 256 ];

		private bool unpackedMode;
		private bool needsPack;

		public PngWriter ( Stream outputStream, ImageInfo imgInfo )
			: this ( outputStream, imgInfo, "[NO FILENAME AVAILABLE]" )
		{
		}

		public PngWriter ( Stream outputStream, ImageInfo imgInfo,
				String filename )
		{
			this.filename = ( filename == null ) ? "" : filename;
			this.outputStream = outputStream;
			this.ImgInfo = imgInfo;
			this.CompLevel = 6;
			this.ShouldCloseStream = true;
			this.IdatMaxSize = 0;
			this.CompressionStrategy = EDeflateCompressStrategy.Filtered;
			rowb = new byte [ imgInfo.BytesPerRow + 1 ];
			rowbprev = new byte [ rowb.Length ];
			rowbfilter = new byte [ rowb.Length ];
			chunksList = new ChunksListForWrite ( ImgInfo );
			metadata = new PngMetadata ( chunksList );
			filterStrat = new FilterWriteStrategy ( ImgInfo, FilterType.FILTER_DEFAULT );
			unpackedMode = false;
			needsPack = unpackedMode && imgInfo.Packed;
		}

		private void init ()
		{
			datStream = new PngIDatChunkOutputStream ( this.outputStream, this.IdatMaxSize );
			datStreamDeflated = ZlibStreamFactory.createZlibOutputStream ( datStream, this.CompLevel, this.CompressionStrategy, true );
			WriteSignatureAndIHDR ();
			WriteFirstChunks ();
		}

		private void reportResultsForFilter ( int rown, FilterType type, bool tentative )
		{
			for ( int i = 0; i < histox.Length; i++ )
				histox [ i ] = 0;
			int s = 0, v;
			for ( int i = 1; i <= ImgInfo.BytesPerRow; i++ )
			{
				v = rowbfilter [ i ];
				if ( v < 0 )
					s -= ( int ) v;
				else
					s += ( int ) v;
				histox [ v & 0xFF ]++;
			}
			filterStrat.fillResultsForFilter ( rown, type, s, histox, tentative );
		}

		private void WriteEndChunk ()
		{
			PngChunkIEND c = new PngChunkIEND ( ImgInfo );
			c.CreateRawChunk ().WriteChunk ( outputStream );
		}

		private void WriteFirstChunks ()
		{
			int nw = 0;
			CurrentChunkGroup = ChunksList.CHUNK_GROUP_1_AFTERIDHR;
			nw = chunksList.writeChunks ( outputStream, CurrentChunkGroup );
			CurrentChunkGroup = ChunksList.CHUNK_GROUP_2_PLTE;
			nw = chunksList.writeChunks ( outputStream, CurrentChunkGroup );
			if ( nw > 0 && ImgInfo.Greyscale )
				throw new PngjOutputException ( "cannot write palette for this format" );
			if ( nw == 0 && ImgInfo.Indexed )
				throw new PngjOutputException ( "missing palette" );
			CurrentChunkGroup = ChunksList.CHUNK_GROUP_3_AFTERPLTE;
			nw = chunksList.writeChunks ( outputStream, CurrentChunkGroup );
			CurrentChunkGroup = ChunksList.CHUNK_GROUP_4_IDAT;
		}

		private void WriteLastChunks ()
		{
			CurrentChunkGroup = ChunksList.CHUNK_GROUP_5_AFTERIDAT;
			chunksList.writeChunks ( outputStream, CurrentChunkGroup );
			List<PngChunk> pending = chunksList.GetQueuedChunks ();
			if ( pending.Count > 0 )
				throw new PngjOutputException ( pending.Count + " chunks were not written! Eg: " + pending [ 0 ].ToString () );
			CurrentChunkGroup = ChunksList.CHUNK_GROUP_6_END;
		}

		private void WriteSignatureAndIHDR ()
		{
			CurrentChunkGroup = ChunksList.CHUNK_GROUP_0_IDHR;
			PngHelperInternal.WriteBytes ( outputStream, Hjg.Pngcs.PngHelperInternal.PNG_ID_SIGNATURE ); // signature
			PngChunkIHDR ihdr = new PngChunkIHDR ( ImgInfo );
			ihdr.Cols = ImgInfo.Cols;
			ihdr.Rows = ImgInfo.Rows;
			ihdr.Bitspc = ImgInfo.BitDepth;
			int colormodel = 0;
			if ( ImgInfo.Alpha )
				colormodel += 0x04;
			if ( ImgInfo.Indexed )
				colormodel += 0x01;
			if ( !ImgInfo.Greyscale )
				colormodel += 0x02;
			ihdr.Colormodel = colormodel;
			ihdr.Compmeth = 0;
			ihdr.Filmeth = 0;
			ihdr.Interlaced = 0;
			ihdr.CreateRawChunk ().WriteChunk ( outputStream );
		}

		protected void encodeRowFromByte ( byte [] row )
		{
			if ( row.Length == ImgInfo.SamplesPerRowPacked && !needsPack )
			{
				int j = 1;
				if ( ImgInfo.BitDepth <= 8 )
				{
					foreach ( byte x in row )
					{
						rowb [ j++ ] = x;
					}
				}
				else
				{
					foreach ( byte x in row )
					{
						rowb [ j ] = x;
						j += 2;
					}
				}
			}
			else
			{
				if ( row.Length >= ImgInfo.SamplesPerRow && needsPack )
					ImageLine.packInplaceByte ( ImgInfo, row, row, false );
				if ( ImgInfo.BitDepth <= 8 )
				{
					for ( int i = 0, j = 1; i < ImgInfo.SamplesPerRowPacked; i++ )
					{
						rowb [ j++ ] = row [ i ];
					}
				}
				else
				{
					for ( int i = 0, j = 1; i < ImgInfo.SamplesPerRowPacked; i++ )
					{
						rowb [ j++ ] = row [ i ];
						rowb [ j++ ] = 0;
					}
				}

			}
		}

		protected void encodeRowFromInt ( int [] row )
		{
			if ( row.Length == ImgInfo.SamplesPerRowPacked && !needsPack )
			{
				int j = 1;
				if ( ImgInfo.BitDepth <= 8 )
				{
					foreach ( int x in row )
					{
						rowb [ j++ ] = ( byte ) x;
					}
				}
				else
				{
					foreach ( int x in row )
					{
						rowb [ j++ ] = ( byte ) ( x >> 8 );
						rowb [ j++ ] = ( byte ) ( x );
					}
				}
			}
			else
			{
				if ( row.Length >= ImgInfo.SamplesPerRow && needsPack )
					ImageLine.packInplaceInt ( ImgInfo, row, row, false );
				if ( ImgInfo.BitDepth <= 8 )
				{
					for ( int i = 0, j = 1; i < ImgInfo.SamplesPerRowPacked; i++ )
					{
						rowb [ j++ ] = ( byte ) ( row [ i ] );
					}
				}
				else
				{
					for ( int i = 0, j = 1; i < ImgInfo.SamplesPerRowPacked; i++ )
					{
						rowb [ j++ ] = ( byte ) ( row [ i ] >> 8 );
						rowb [ j++ ] = ( byte ) ( row [ i ] );
					}
				}

			}
		}

		private void FilterRow ( int rown )
		{
			if ( filterStrat.shouldTestAll ( rown ) )
			{
				FilterRowNone ();
				reportResultsForFilter ( rown, FilterType.FILTER_NONE, true );
				FilterRowSub ();
				reportResultsForFilter ( rown, FilterType.FILTER_SUB, true );
				FilterRowUp ();
				reportResultsForFilter ( rown, FilterType.FILTER_UP, true );
				FilterRowAverage ();
				reportResultsForFilter ( rown, FilterType.FILTER_AVERAGE, true );
				FilterRowPaeth ();
				reportResultsForFilter ( rown, FilterType.FILTER_PAETH, true );
			}
			FilterType filterType = filterStrat.gimmeFilterType ( rown, true );
			rowbfilter [ 0 ] = ( byte ) ( int ) filterType;
			switch ( filterType )
			{
				case Hjg.Pngcs.FilterType.FILTER_NONE:
					FilterRowNone ();
					break;
				case Hjg.Pngcs.FilterType.FILTER_SUB:
					FilterRowSub ();
					break;
				case Hjg.Pngcs.FilterType.FILTER_UP:
					FilterRowUp ();
					break;
				case Hjg.Pngcs.FilterType.FILTER_AVERAGE:
					FilterRowAverage ();
					break;
				case Hjg.Pngcs.FilterType.FILTER_PAETH:
					FilterRowPaeth ();
					break;
				default:
					throw new PngjOutputException ( "Filter type " + filterType + " not implemented" );
			}
			reportResultsForFilter ( rown, filterType, false );
		}

		private void prepareEncodeRow ( int rown )
		{
			if ( datStream == null )
				init ();
			rowNum++;
			if ( rown >= 0 && rowNum != rown )
				throw new PngjOutputException ( "rows must be written in order: expected:" + rowNum
						+ " passed:" + rown );
			byte [] tmp = rowb;
			rowb = rowbprev;
			rowbprev = tmp;
		}

		private void filterAndSend ( int rown )
		{
			FilterRow ( rown );
			datStreamDeflated.Write ( rowbfilter, 0, ImgInfo.BytesPerRow + 1 );
		}

		private void FilterRowAverage ()
		{
			int i, j, imax;
			imax = ImgInfo.BytesPerRow;
			for ( j = 1 - ImgInfo.BytesPixel, i = 1; i <= imax; i++, j++ )
			{
				rowbfilter [ i ] = ( byte ) ( rowb [ i ] - ( ( rowbprev [ i ] ) + ( j > 0 ? rowb [ j ] : 0 ) ) / 2 );
			}
		}

		private void FilterRowNone ()
		{
			for ( int i = 1; i <= ImgInfo.BytesPerRow; i++ )
			{
				rowbfilter [ i ] = ( byte ) rowb [ i ];
			}
		}

		private void FilterRowPaeth ()
		{
			int i, j, imax;
			imax = ImgInfo.BytesPerRow;
			for ( j = 1 - ImgInfo.BytesPixel, i = 1; i <= imax; i++, j++ )
			{
				rowbfilter [ i ] = ( byte ) ( rowb [ i ] - PngHelperInternal.FilterPaethPredictor ( j > 0 ? rowb [ j ] : 0,
						rowbprev [ i ], j > 0 ? rowbprev [ j ] : 0 ) );
			}
		}

		private void FilterRowSub ()
		{
			int i, j;
			for ( i = 1; i <= ImgInfo.BytesPixel; i++ )
			{
				rowbfilter [ i ] = ( byte ) rowb [ i ];
			}
			for ( j = 1, i = ImgInfo.BytesPixel + 1; i <= ImgInfo.BytesPerRow; i++, j++ )
			{
				rowbfilter [ i ] = ( byte ) ( rowb [ i ] - rowb [ j ] );
			}
		}

		private void FilterRowUp ()
		{
			for ( int i = 1; i <= ImgInfo.BytesPerRow; i++ )
			{
				rowbfilter [ i ] = ( byte ) ( rowb [ i ] - rowbprev [ i ] );
			}
		}

		private long SumRowbfilter ()
		{
			long s = 0;
			for ( int i = 1; i <= ImgInfo.BytesPerRow; i++ )
				if ( rowbfilter [ i ] < 0 )
					s -= ( long ) rowbfilter [ i ];
				else
					s += ( long ) rowbfilter [ i ];
			return s;
		}

		private void CopyChunks ( PngReader reader, int copy_mask, bool onlyAfterIdat )
		{
			bool idatDone = CurrentChunkGroup >= ChunksList.CHUNK_GROUP_4_IDAT;
			if ( onlyAfterIdat && reader.CurrentChunkGroup < ChunksList.CHUNK_GROUP_6_END ) throw new PngjException ( "tried to copy last chunks but reader has not ended" );
			foreach ( PngChunk chunk in reader.GetChunksList ().GetChunks () )
			{
				int group = chunk.ChunkGroup;
				if ( group < ChunksList.CHUNK_GROUP_4_IDAT && idatDone )
					continue;
				bool copy = false;
				if ( chunk.Crit )
				{
					if ( chunk.Id.Equals ( ChunkHelper.PLTE ) )
					{
						if ( ImgInfo.Indexed && ChunkHelper.maskMatch ( copy_mask, ChunkCopyBehaviour.COPY_PALETTE ) )
							copy = true;
						if ( !ImgInfo.Greyscale && ChunkHelper.maskMatch ( copy_mask, ChunkCopyBehaviour.COPY_ALL ) )
							copy = true;
					}
				}
				else
				{
					bool text = ( chunk is PngChunkTextVar );
					bool safe = chunk.Safe;
					if ( ChunkHelper.maskMatch ( copy_mask, ChunkCopyBehaviour.COPY_ALL ) )
						copy = true;
					if ( safe && ChunkHelper.maskMatch ( copy_mask, ChunkCopyBehaviour.COPY_ALL_SAFE ) )
						copy = true;
					if ( chunk.Id.Equals ( ChunkHelper.tRNS )
							&& ChunkHelper.maskMatch ( copy_mask, ChunkCopyBehaviour.COPY_TRANSPARENCY ) )
						copy = true;
					if ( chunk.Id.Equals ( ChunkHelper.pHYs ) && ChunkHelper.maskMatch ( copy_mask, ChunkCopyBehaviour.COPY_PHYS ) )
						copy = true;
					if ( text && ChunkHelper.maskMatch ( copy_mask, ChunkCopyBehaviour.COPY_TEXTUAL ) )
						copy = true;
					if ( ChunkHelper.maskMatch ( copy_mask, ChunkCopyBehaviour.COPY_ALMOSTALL )
							&& !( ChunkHelper.IsUnknown ( chunk ) || text || chunk.Id.Equals ( ChunkHelper.hIST ) || chunk.Id
									.Equals ( ChunkHelper.tIME ) ) )
						copy = true;
					if ( chunk is PngChunkSkipped )
						copy = false;
				}
				if ( copy )
				{
					chunksList.Queue ( PngChunk.CloneChunk ( chunk, ImgInfo ) );
				}
			}
		}

		public void CopyChunksFirst ( PngReader reader, int copy_mask )
		{
			CopyChunks ( reader, copy_mask, false );
		}

		public void CopyChunksLast ( PngReader reader, int copy_mask )
		{
			CopyChunks ( reader, copy_mask, true );
		}

		public double ComputeCompressionRatio ()
		{
			if ( CurrentChunkGroup < ChunksList.CHUNK_GROUP_6_END )
				throw new PngjException ( "must be called after End()" );
			double compressed = ( double ) datStream.GetCountFlushed ();
			double raw = ( ImgInfo.BytesPerRow + 1 ) * ImgInfo.Rows;
			return compressed / raw;
		}

		public void End ()
		{
			if ( rowNum != ImgInfo.Rows - 1 )
				throw new PngjOutputException ( "all rows have not been written" );
			try
			{
				datStreamDeflated.Dispose ();
				datStream.Dispose ();
				WriteLastChunks ();
				WriteEndChunk ();
				if ( this.ShouldCloseStream )
					outputStream.Dispose ();
			}
			catch ( IOException e )
			{
				throw new PngjOutputException ( e );
			}
		}

		public String GetFilename ()
		{
			return filename;
		}

		public void WriteRow ( ImageLine imgline, int rownumber )
		{
			SetUseUnPackedMode ( imgline.SamplesUnpacked );
			if ( imgline.SampleType == ImageLine.ESampleType.INT )
				WriteRowInt ( imgline.Scanline, rownumber );
			else
				WriteRowByte ( imgline.ScanlineB, rownumber );
		}

		public void WriteRow ( int [] newrow )
		{
			WriteRow ( newrow, -1 );
		}

		public void WriteRow ( int [] newrow, int rown )
		{
			WriteRowInt ( newrow, rown );
		}

		public void WriteRowInt ( int [] newrow, int rown )
		{
			prepareEncodeRow ( rown );
			encodeRowFromInt ( newrow );
			filterAndSend ( rown );
		}

		public void WriteRowByte ( byte [] newrow, int rown )
		{
			prepareEncodeRow ( rown );
			encodeRowFromByte ( newrow );
			filterAndSend ( rown );
		}

		public void WriteRowsInt ( int [] [] image )
		{
			for ( int i = 0; i < ImgInfo.Rows; i++ )
				WriteRowInt ( image [ i ], i );
		}

		public void WriteRowsByte ( byte [] [] image )
		{
			for ( int i = 0; i < ImgInfo.Rows; i++ )
				WriteRowByte ( image [ i ], i );
		}

		public PngMetadata GetMetadata ()
		{
			return metadata;
		}

		public ChunksListForWrite GetChunksList ()
		{
			return chunksList;
		}

		public void SetFilterType ( FilterType filterType )
		{
			filterStrat = new FilterWriteStrategy ( ImgInfo, filterType );
		}

		public bool IsUnpackedMode ()
		{
			return unpackedMode;
		}

		public void SetUseUnPackedMode ( bool useUnpackedMode )
		{
			this.unpackedMode = useUnpackedMode;
			needsPack = unpackedMode && ImgInfo.Packed;
		}
	}
}
