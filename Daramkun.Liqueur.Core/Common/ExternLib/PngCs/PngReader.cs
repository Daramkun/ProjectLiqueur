namespace Hjg.Pngcs
{
	using Hjg.Pngcs.Chunks;
	using Hjg.Pngcs.Zlib;
	using System.Collections.Generic;
	using System.IO;
	using System;
	using System.Diagnostics;

	internal class PngReader : IDisposable
	{
		public ImageInfo ImgInfo { get; private set; }

		protected readonly String filename;

		public ChunkLoadBehaviour ChunkLoadBehaviour { get; set; }

		public bool ShouldCloseStream { get; set; }

		public long MaxBytesMetadata { get; set; }

		public long MaxTotalBytesRead { get; set; }

		public int SkipChunkMaxSize { get; set; }

		public String [] SkipChunkIds { get; set; }

		private Dictionary<string, int> skipChunkIdsSet = null;

		private readonly PngMetadata metadata;
		private readonly ChunksList chunksList;

		protected ImageLine imgLine;

		protected byte [] rowb;
		protected byte [] rowbprev;
		protected byte [] rowbfilter;

		public readonly bool interlaced;
		private readonly PngDeinterlacer deinterlacer;

		private bool crcEnabled = true;

		private bool unpackedMode = false;

		public int CurrentChunkGroup { get; private set; }
		protected int rowNum = -1;
		private long offset = 0;
		private int bytesChunksLoaded = 0;

		private readonly Stream inputStream;
		internal AZlibInputStream idatIstream;
		internal PngIDatChunkInputStream iIdatCstream;

		protected Adler32 crctest;

		public PngReader ( Stream inputStream )
			: this ( inputStream, "[NO FILENAME AVAILABLE]" )
		{
		}

		public PngReader ( Stream inputStream, String filename )
		{
			this.filename = ( filename == null ) ? "" : filename;
			this.inputStream = inputStream;
			this.chunksList = new ChunksList ( null );
			this.metadata = new PngMetadata ( chunksList );
			this.offset = 0;
			this.CurrentChunkGroup = -1;
			this.ShouldCloseStream = true;
			this.MaxBytesMetadata = 5 * 1024 * 1024;
			this.MaxTotalBytesRead = 200 * 1024 * 1024;
			this.SkipChunkMaxSize = 2 * 1024 * 1024;
			this.SkipChunkIds = new string [] { "fdAT" };
			this.ChunkLoadBehaviour = Hjg.Pngcs.Chunks.ChunkLoadBehaviour.LOAD_CHUNK_ALWAYS;
			byte [] pngid = new byte [ 8 ];
			PngHelperInternal.ReadBytes ( inputStream, pngid, 0, pngid.Length );
			offset += pngid.Length;
			if ( !PngCsUtils.arraysEqual ( pngid, PngHelperInternal.PNG_ID_SIGNATURE ) )
				throw new PngjInputException ( "Bad PNG signature" );
			CurrentChunkGroup = ChunksList.CHUNK_GROUP_0_IDHR;
			int clen = PngHelperInternal.ReadInt4 ( inputStream );
			offset += 4;
			if ( clen != 13 )
				throw new Exception ( "IDHR chunk len != 13 ?? " + clen );
			byte [] chunkid = new byte [ 4 ];
			PngHelperInternal.ReadBytes ( inputStream, chunkid, 0, 4 );
			if ( !PngCsUtils.arraysEqual4 ( chunkid, ChunkHelper.b_IHDR ) )
				throw new PngjInputException ( "IHDR not found as first chunk??? ["
						+ ChunkHelper.ToString ( chunkid ) + "]" );
			offset += 4;
			PngChunkIHDR ihdr = ( PngChunkIHDR ) ReadChunk ( chunkid, clen, false );
			bool alpha = ( ihdr.Colormodel & 0x04 ) != 0;
			bool palette = ( ihdr.Colormodel & 0x01 ) != 0;
			bool grayscale = ( ihdr.Colormodel == 0 || ihdr.Colormodel == 4 );
			ImgInfo = new ImageInfo ( ihdr.Cols, ihdr.Rows, ihdr.Bitspc, alpha, grayscale, palette );
			rowb = new byte [ ImgInfo.BytesPerRow + 1 ];
			rowbprev = new byte [ rowb.Length ];
			rowbfilter = new byte [ rowb.Length ];
			interlaced = ihdr.Interlaced == 1;
			deinterlacer = interlaced ? new PngDeinterlacer ( ImgInfo ) : null;
			if ( ihdr.Filmeth != 0 || ihdr.Compmeth != 0 || ( ihdr.Interlaced & 0xFFFE ) != 0 )
				throw new PngjInputException ( "compmethod or filtermethod or interlaced unrecognized" );
			if ( ihdr.Colormodel < 0 || ihdr.Colormodel > 6 || ihdr.Colormodel == 1
					|| ihdr.Colormodel == 5 )
				throw new PngjInputException ( "Invalid colormodel " + ihdr.Colormodel );
			if ( ihdr.Bitspc != 1 && ihdr.Bitspc != 2 && ihdr.Bitspc != 4 && ihdr.Bitspc != 8
					&& ihdr.Bitspc != 16 )
				throw new PngjInputException ( "Invalid bit depth " + ihdr.Bitspc );
		}

		~PngReader () { Dispose ( false ); }

		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				
			}
		}

		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		private bool FirstChunksNotYetRead ()
		{
			return CurrentChunkGroup < ChunksList.CHUNK_GROUP_1_AFTERIDHR;
		}

		private void ReadLastAndClose ()
		{
			if ( CurrentChunkGroup < ChunksList.CHUNK_GROUP_5_AFTERIDAT )
			{
				try
				{
					idatIstream.Dispose ();
				}
				catch ( Exception ) { }
				ReadLastChunks ();
			}
			Close ();
		}

		private void Close ()
		{
			if ( CurrentChunkGroup < ChunksList.CHUNK_GROUP_6_END )
			{
				try
				{
					idatIstream.Dispose ();
				}
				catch ( Exception )
				{
				}
				CurrentChunkGroup = ChunksList.CHUNK_GROUP_6_END;
			}
			if ( ShouldCloseStream )
				inputStream.Dispose ();
		}

		private void UnfilterRow ( int nbytes )
		{
			int ftn = rowbfilter [ 0 ];
			FilterType ft = ( FilterType ) ftn;
			switch ( ft )
			{
				case Hjg.Pngcs.FilterType.FILTER_NONE:
					UnfilterRowNone ( nbytes );
					break;
				case Hjg.Pngcs.FilterType.FILTER_SUB:
					UnfilterRowSub ( nbytes );
					break;
				case Hjg.Pngcs.FilterType.FILTER_UP:
					UnfilterRowUp ( nbytes );
					break;
				case Hjg.Pngcs.FilterType.FILTER_AVERAGE:
					UnfilterRowAverage ( nbytes );
					break;
				case Hjg.Pngcs.FilterType.FILTER_PAETH:
					UnfilterRowPaeth ( nbytes );
					break;
				default:
					throw new PngjInputException ( "Filter type " + ftn + " not implemented" );
			}
			if ( crctest != null )
				crctest.Update ( rowb, 1, nbytes );
		}


		private void UnfilterRowAverage ( int nbytes )
		{
			int i, j, x;
			for ( j = 1 - ImgInfo.BytesPixel, i = 1; i <= nbytes; i++, j++ )
			{
				x = ( j > 0 ) ? rowb [ j ] : 0;
				rowb [ i ] = ( byte ) ( rowbfilter [ i ] + ( x + ( rowbprev [ i ] & 0xFF ) ) / 2 );
			}
		}

		private void UnfilterRowNone ( int nbytes )
		{
			for ( int i = 1; i <= nbytes; i++ )
			{
				rowb [ i ] = ( byte ) ( rowbfilter [ i ] );
			}
		}

		private void UnfilterRowPaeth ( int nbytes )
		{
			int i, j, x, y;
			for ( j = 1 - ImgInfo.BytesPixel, i = 1; i <= nbytes; i++, j++ )
			{
				x = ( j > 0 ) ? rowb [ j ] : 0;
				y = ( j > 0 ) ? rowbprev [ j ] : 0;
				rowb [ i ] = ( byte ) ( rowbfilter [ i ] + PngHelperInternal.FilterPaethPredictor ( x, rowbprev [ i ], y ) );
			}
		}

		private void UnfilterRowSub ( int nbytes )
		{
			int i, j;
			for ( i = 1; i <= ImgInfo.BytesPixel; i++ )
			{
				rowb [ i ] = ( byte ) ( rowbfilter [ i ] );
			}
			for ( j = 1, i = ImgInfo.BytesPixel + 1; i <= nbytes; i++, j++ )
			{
				rowb [ i ] = ( byte ) ( rowbfilter [ i ] + rowb [ j ] );
			}
		}

		private void UnfilterRowUp ( int nbytes )
		{
			for ( int i = 1; i <= nbytes; i++ )
			{
				rowb [ i ] = ( byte ) ( rowbfilter [ i ] + rowbprev [ i ] );
			}
		}

		private void ReadFirstChunks ()
		{
			if ( !FirstChunksNotYetRead () )
				return;
			int clen = 0;
			bool found = false;
			byte [] chunkid = new byte [ 4 ];
			this.CurrentChunkGroup = ChunksList.CHUNK_GROUP_1_AFTERIDHR;
			while ( !found )
			{
				clen = PngHelperInternal.ReadInt4 ( inputStream );
				offset += 4;
				if ( clen < 0 )
					break;
				PngHelperInternal.ReadBytes ( inputStream, chunkid, 0, 4 );
				offset += 4;
				if ( PngCsUtils.arraysEqual4 ( chunkid, Hjg.Pngcs.Chunks.ChunkHelper.b_IDAT ) )
				{
					found = true;
					this.CurrentChunkGroup = ChunksList.CHUNK_GROUP_4_IDAT;
					chunksList.AppendReadChunk ( new PngChunkIDAT ( ImgInfo, clen, offset - 8 ), CurrentChunkGroup );
					break;
				}
				else if ( PngCsUtils.arraysEqual4 ( chunkid, Hjg.Pngcs.Chunks.ChunkHelper.b_IEND ) )
				{
					throw new PngjInputException ( "END chunk found before image data (IDAT) at offset=" + offset );
				}
				String chunkids = ChunkHelper.ToString ( chunkid );
				if ( chunkids.Equals ( ChunkHelper.PLTE ) )
					this.CurrentChunkGroup = ChunksList.CHUNK_GROUP_2_PLTE;
				ReadChunk ( chunkid, clen, false );
				if ( chunkids.Equals ( ChunkHelper.PLTE ) )
					this.CurrentChunkGroup = ChunksList.CHUNK_GROUP_3_AFTERPLTE;
			}
			int idatLen = found ? clen : -1;
			if ( idatLen < 0 )
				throw new PngjInputException ( "first idat chunk not found!" );
			iIdatCstream = new PngIDatChunkInputStream ( inputStream, idatLen, offset );
			idatIstream = ZlibStreamFactory.createZlibInputStream ( iIdatCstream, true );
			if ( !crcEnabled )
				iIdatCstream.DisableCrcCheck ();
		}

		private void ReadLastChunks ()
		{
			CurrentChunkGroup = ChunksList.CHUNK_GROUP_5_AFTERIDAT;
			if ( !iIdatCstream.IsEnded () )
				iIdatCstream.ForceChunkEnd ();
			int clen = iIdatCstream.GetLenLastChunk ();
			byte [] chunkid = iIdatCstream.GetIdLastChunk ();
			bool endfound = false;
			bool first = true;
			bool skip = false;
			while ( !endfound )
			{
				skip = false;
				if ( !first )
				{
					clen = PngHelperInternal.ReadInt4 ( inputStream );
					offset += 4;
					if ( clen < 0 )
						throw new PngjInputException ( "bad len " + clen );
					PngHelperInternal.ReadBytes ( inputStream, chunkid, 0, 4 );
					offset += 4;
				}
				first = false;
				if ( PngCsUtils.arraysEqual4 ( chunkid, ChunkHelper.b_IDAT ) )
				{
					skip = true;
				}
				else if ( PngCsUtils.arraysEqual4 ( chunkid, ChunkHelper.b_IEND ) )
				{
					CurrentChunkGroup = ChunksList.CHUNK_GROUP_6_END;
					endfound = true;
				}
				ReadChunk ( chunkid, clen, skip );
			}
			if ( !endfound )
				throw new PngjInputException ( "end chunk not found - offset=" + offset );
		}

		private PngChunk ReadChunk ( byte [] chunkid, int clen, bool skipforced )
		{
			if ( clen < 0 ) throw new PngjInputException ( "invalid chunk lenght: " + clen );
			if ( skipChunkIdsSet == null && CurrentChunkGroup > ChunksList.CHUNK_GROUP_0_IDHR )
			{
				skipChunkIdsSet = new Dictionary<string, int> ();
				if ( SkipChunkIds != null )
					foreach ( string id in SkipChunkIds ) skipChunkIdsSet.Add ( id, 1 );
			}

			String chunkidstr = ChunkHelper.ToString ( chunkid );
			PngChunk pngChunk = null;
			bool critical = ChunkHelper.IsCritical ( chunkidstr );
			bool skip = skipforced;
			if ( MaxTotalBytesRead > 0 && clen + offset > MaxTotalBytesRead )
				throw new PngjInputException ( "Maximum total bytes to read exceeeded: " + MaxTotalBytesRead + " offset:"
						+ offset + " clen=" + clen );
			if ( CurrentChunkGroup > ChunksList.CHUNK_GROUP_0_IDHR && !ChunkHelper.IsCritical ( chunkidstr ) )
				skip = skip || ( SkipChunkMaxSize > 0 && clen >= SkipChunkMaxSize ) || skipChunkIdsSet.ContainsKey ( chunkidstr )
						|| ( MaxBytesMetadata > 0 && clen > MaxBytesMetadata - bytesChunksLoaded )
						|| !ChunkHelper.ShouldLoad ( chunkidstr, ChunkLoadBehaviour );

			if ( skip )
			{
				PngHelperInternal.SkipBytes ( inputStream, clen );
				PngHelperInternal.ReadInt4 ( inputStream );
				pngChunk = new PngChunkSkipped ( chunkidstr, ImgInfo, clen );
			}
			else
			{
				ChunkRaw chunk = new ChunkRaw ( clen, chunkid, true );
				chunk.ReadChunkData ( inputStream, crcEnabled || critical );
				pngChunk = PngChunk.Factory ( chunk, ImgInfo );
				if ( !pngChunk.Crit )
				{
					bytesChunksLoaded += chunk.Length;
				}

			}
			pngChunk.Offset = offset - 8L;
			chunksList.AppendReadChunk ( pngChunk, CurrentChunkGroup );
			offset += clen + 4L;
			return pngChunk;
		}

		internal void logWarn ( String warn )
		{
			Debug.WriteLine ( warn );
		}

		public ChunksList GetChunksList ()
		{
			if ( FirstChunksNotYetRead () )
				ReadFirstChunks ();
			return chunksList;
		}

		public PngMetadata GetMetadata ()
		{
			if ( FirstChunksNotYetRead () )
				ReadFirstChunks ();
			return metadata;
		}

		public ImageLine ReadRow ( int nrow )
		{
			return imgLine == null || imgLine.SampleType != ImageLine.ESampleType.BYTE ? ReadRowInt ( nrow ) : ReadRowByte ( nrow );
		}

		public ImageLine ReadRowInt ( int nrow )
		{
			if ( imgLine == null )
				imgLine = new ImageLine ( ImgInfo, ImageLine.ESampleType.INT, unpackedMode );
			if ( imgLine.Rown == nrow )
				return imgLine;
			ReadRowInt ( imgLine.Scanline, nrow );
			imgLine.FilterUsed = ( FilterType ) rowbfilter [ 0 ];
			imgLine.Rown = nrow;
			return imgLine;
		}

		public ImageLine ReadRowByte ( int nrow )
		{
			if ( imgLine == null )
				imgLine = new ImageLine ( ImgInfo, ImageLine.ESampleType.BYTE, unpackedMode );
			if ( imgLine.Rown == nrow )
				return imgLine;
			ReadRowByte ( imgLine.ScanlineB, nrow );
			imgLine.FilterUsed = ( FilterType ) rowbfilter [ 0 ];
			imgLine.Rown = nrow;
			return imgLine;
		}

		public int [] ReadRow ( int [] buffer, int nrow )
		{
			return ReadRowInt ( buffer, nrow );
		}

		public int [] ReadRowInt ( int [] buffer, int nrow )
		{
			if ( buffer == null )
				buffer = new int [ unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked ];
			if ( !interlaced )
			{
				if ( nrow <= rowNum )
					throw new PngjInputException ( "rows must be read in increasing order: " + nrow );
				int bytesread = 0;
				while ( rowNum < nrow )
					bytesread = ReadRowRaw ( rowNum + 1 );
				decodeLastReadRowToInt ( buffer, bytesread );
			}
			else
			{
				if ( deinterlacer.getImageInt () == null )
					deinterlacer.setImageInt ( ReadRowsInt ().Scanlines );
				Array.Copy ( deinterlacer.getImageInt () [ nrow ], 0, buffer, 0, unpackedMode ? ImgInfo.SamplesPerRow
						: ImgInfo.SamplesPerRowPacked );
			}
			return buffer;
		}

		public byte [] ReadRowByte ( byte [] buffer, int nrow )
		{
			if ( buffer == null )
				buffer = new byte [ unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked ];
			if ( !interlaced )
			{
				if ( nrow <= rowNum )
					throw new PngjInputException ( "rows must be read in increasing order: " + nrow );
				int bytesread = 0;
				while ( rowNum < nrow )
					bytesread = ReadRowRaw ( rowNum + 1 );
				decodeLastReadRowToByte ( buffer, bytesread );
			}
			else
			{ // interlaced
				if ( deinterlacer.getImageByte () == null )
					deinterlacer.setImageByte ( ReadRowsByte ().ScanlinesB );
				Array.Copy ( deinterlacer.getImageByte () [ nrow ], 0, buffer, 0, unpackedMode ? ImgInfo.SamplesPerRow
						: ImgInfo.SamplesPerRowPacked );
			}
			return buffer;
		}

		[Obsolete ( "GetRow is deprecated,  use ReadRow/ReadRowInt/ReadRowByte instead." )]
		public ImageLine GetRow ( int nrow )
		{
			return ReadRow ( nrow );
		}

		private void decodeLastReadRowToInt ( int [] buffer, int bytesRead )
		{
			if ( ImgInfo.BitDepth <= 8 )
			{
				for ( int i = 0, j = 1; i < bytesRead; i++ )
					buffer [ i ] = ( rowb [ j++ ] );
			}
			else
			{
				for ( int i = 0, j = 1; j < bytesRead; i++ )
					buffer [ i ] = ( rowb [ j++ ] << 8 ) + rowb [ j++ ];
			}
			if ( ImgInfo.Packed && unpackedMode )
				ImageLine.unpackInplaceInt ( ImgInfo, buffer, buffer, false );
		}

		private void decodeLastReadRowToByte ( byte [] buffer, int bytesRead )
		{
			if ( ImgInfo.BitDepth <= 8 )
			{
				Array.Copy ( rowb, 1, buffer, 0, bytesRead );
			}
			else
			{
				for ( int i = 0, j = 1; j < bytesRead; i++, j += 2 )
					buffer [ i ] = rowb [ j ];
			}
			if ( ImgInfo.Packed && unpackedMode )
				ImageLine.unpackInplaceByte ( ImgInfo, buffer, buffer, false );
		}


		public ImageLines ReadRowsInt ( int rowOffset, int nRows, int rowStep )
		{
			if ( nRows < 0 )
				nRows = ( ImgInfo.Rows - rowOffset ) / rowStep;
			if ( rowStep < 1 || rowOffset < 0 || nRows * rowStep + rowOffset > ImgInfo.Rows )
				throw new PngjInputException ( "bad args" );
			ImageLines imlines = new ImageLines ( ImgInfo, ImageLine.ESampleType.INT, unpackedMode, rowOffset, nRows, rowStep );
			if ( !interlaced )
			{
				for ( int j = 0; j < ImgInfo.Rows; j++ )
				{
					int bytesread = ReadRowRaw ( j );
					int mrow = imlines.ImageRowToMatrixRowStrict ( j );
					if ( mrow >= 0 )
						decodeLastReadRowToInt ( imlines.Scanlines [ mrow ], bytesread );
				}
			}
			else
			{
				int [] buf = new int [ unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked ];
				for ( int p = 1; p <= 7; p++ )
				{
					deinterlacer.setPass ( p );
					for ( int i = 0; i < deinterlacer.getRows (); i++ )
					{
						int bytesread = ReadRowRaw ( i );
						int j = deinterlacer.getCurrRowReal ();
						int mrow = imlines.ImageRowToMatrixRowStrict ( j );
						if ( mrow >= 0 )
						{
							decodeLastReadRowToInt ( buf, bytesread );
							deinterlacer.deinterlaceInt ( buf, imlines.Scanlines [ mrow ], !unpackedMode );
						}
					}
				}
			}
			End ();
			return imlines;
		}

		public ImageLines ReadRowsInt ()
		{
			return ReadRowsInt ( 0, ImgInfo.Rows, 1 );
		}

		public ImageLines ReadRowsByte ( int rowOffset, int nRows, int rowStep )
		{
			if ( nRows < 0 )
				nRows = ( ImgInfo.Rows - rowOffset ) / rowStep;
			if ( rowStep < 1 || rowOffset < 0 || nRows * rowStep + rowOffset > ImgInfo.Rows )
				throw new PngjInputException ( "bad args" );
			ImageLines imlines = new ImageLines ( ImgInfo, ImageLine.ESampleType.BYTE, unpackedMode, rowOffset, nRows, rowStep );
			if ( !interlaced )
			{
				for ( int j = 0; j < ImgInfo.Rows; j++ )
				{
					int bytesread = ReadRowRaw ( j );
					int mrow = imlines.ImageRowToMatrixRowStrict ( j );
					if ( mrow >= 0 )
						decodeLastReadRowToByte ( imlines.ScanlinesB [ mrow ], bytesread );
				}
			}
			else
			{
				byte [] buf = new byte [ unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked ];
				for ( int p = 1; p <= 7; p++ )
				{
					deinterlacer.setPass ( p );
					for ( int i = 0; i < deinterlacer.getRows (); i++ )
					{
						int bytesread = ReadRowRaw ( i );
						int j = deinterlacer.getCurrRowReal ();
						int mrow = imlines.ImageRowToMatrixRowStrict ( j );
						if ( mrow >= 0 )
						{
							decodeLastReadRowToByte ( buf, bytesread );
							deinterlacer.deinterlaceByte ( buf, imlines.ScanlinesB [ mrow ], !unpackedMode );
						}
					}
				}
			}
			End ();
			return imlines;
		}

		public ImageLines ReadRowsByte ()
		{
			return ReadRowsByte ( 0, ImgInfo.Rows, 1 );
		}

		private int ReadRowRaw ( int nrow )
		{
			if ( nrow == 0 && FirstChunksNotYetRead () )
				ReadFirstChunks ();
			if ( nrow == 0 && interlaced )
				Array.Clear ( rowb, 0, rowb.Length );

			int bytesRead = ImgInfo.BytesPerRow;
			if ( interlaced )
			{
				if ( nrow < 0 || nrow > deinterlacer.getRows () || ( nrow != 0 && nrow != deinterlacer.getCurrRowSubimg () + 1 ) )
					throw new PngjInputException ( "invalid row in interlaced mode: " + nrow );
				deinterlacer.setRow ( nrow );
				bytesRead = ( ImgInfo.BitspPixel * deinterlacer.getPixelsToRead () + 7 ) / 8;
				if ( bytesRead < 1 )
					throw new PngjExceptionInternal ( "wtf??" );
			}
			else
			{
				if ( nrow < 0 || nrow >= ImgInfo.Rows || nrow != rowNum + 1 )
					throw new PngjInputException ( "invalid row: " + nrow );
			}
			rowNum = nrow;
			byte [] tmp = rowb;
			rowb = rowbprev;
			rowbprev = tmp;
			PngHelperInternal.ReadBytes ( idatIstream, rowbfilter, 0, bytesRead + 1 );
			offset = iIdatCstream.GetOffset ();
			if ( offset < 0 )
				throw new PngjExceptionInternal ( "bad offset ??" + offset );
			if ( MaxTotalBytesRead > 0 && offset >= MaxTotalBytesRead )
				throw new PngjInputException ( "Reading IDAT: Maximum total bytes to read exceeeded: " + MaxTotalBytesRead
						+ " offset:" + offset );
			rowb [ 0 ] = 0;
			UnfilterRow ( bytesRead );
			rowb [ 0 ] = rowbfilter [ 0 ];
			if ( ( rowNum == ImgInfo.Rows - 1 && !interlaced ) || ( interlaced && deinterlacer.isAtLastRow () ) )
				ReadLastAndClose ();
			return bytesRead;
		}

		public void ReadSkippingAllRows ()
		{
			if ( FirstChunksNotYetRead () )
				ReadFirstChunks ();
			iIdatCstream.DisableCrcCheck ();
			try
			{
				int r;
				do
				{
					r = iIdatCstream.Read ( rowbfilter, 0, rowbfilter.Length );
				} while ( r >= 0 );
			}
			catch ( IOException e )
			{
				throw new PngjInputException ( "error in raw read of IDAT", e );
			}
			offset = iIdatCstream.GetOffset ();
			if ( offset < 0 )
				throw new PngjExceptionInternal ( "bad offset ??" + offset );
			if ( MaxTotalBytesRead > 0 && offset >= MaxTotalBytesRead )
				throw new PngjInputException ( "Reading IDAT: Maximum total bytes to read exceeeded: " + MaxTotalBytesRead
						+ " offset:" + offset );
			ReadLastAndClose ();
		}


		public override String ToString ()
		{
			return "filename=" + filename + " " + ImgInfo.ToString ();
		}
		public void End ()
		{
			if ( CurrentChunkGroup < ChunksList.CHUNK_GROUP_6_END )
				Close ();
		}

		public bool IsInterlaced ()
		{
			return interlaced;
		}

		public void SetUnpackedMode ( bool unPackedMode )
		{
			this.unpackedMode = unPackedMode;
		}

		public bool IsUnpackedMode ()
		{
			return unpackedMode;
		}

		public void SetCrcCheckDisabled ()
		{
			crcEnabled = false;
		}

		internal long GetCrctestVal ()
		{
			return crctest.GetValue ();
		}

		internal void InitCrctest ()
		{
			this.crctest = new Adler32 ();
		}

	}
}
