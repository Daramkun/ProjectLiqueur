using Hjg.Pngcs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hjg.Pngcs
{
	internal class ImageLines
	{
		public ImageInfo ImgInfo { get; private set; }
		public ImageLine.ESampleType sampleType { get; private set; }
		public bool SamplesUnpacked { get; private set; }
		public int RowOffset { get; private set; }
		public int Nrows { get; private set; }
		public int RowStep { get; private set; }
		internal readonly int channels;
		internal readonly int bitDepth;
		internal readonly int elementsPerRow;
		public int [] [] Scanlines { get; private set; }
		public byte [] [] ScanlinesB { get; private set; }

		public ImageLines ( global::Hjg.Pngcs.ImageInfo ImgInfo, global::Hjg.Pngcs.ImageLine.ESampleType sampleType, bool unpackedMode, int rowOffset, int nRows, int rowStep )
		{
			this.ImgInfo = ImgInfo;
			channels = ImgInfo.Channels;
			bitDepth = ImgInfo.BitDepth;
			this.sampleType = sampleType;
			this.SamplesUnpacked = unpackedMode || !ImgInfo.Packed;
			this.RowOffset = rowOffset;
			this.Nrows = nRows;
			this.RowStep = rowStep;
			elementsPerRow = unpackedMode ? ImgInfo.SamplesPerRow : ImgInfo.SamplesPerRowPacked;
			if ( sampleType == ImageLine.ESampleType.INT )
			{
				Scanlines = new int [ nRows ] [];
				for ( int i = 0; i < nRows; i++ ) Scanlines [ i ] = new int [ elementsPerRow ];
				ScanlinesB = null;
			}
			else if ( sampleType == ImageLine.ESampleType.BYTE )
			{
				ScanlinesB = new byte [ nRows ] [];
				for ( int i = 0; i < nRows; i++ ) ScanlinesB [ i ] = new byte [ elementsPerRow ];
				Scanlines = null;
			}
			else
				throw new PngjExceptionInternal ( "bad ImageLine initialization" );
		}

		public int ImageRowToMatrixRow ( int imrow )
		{
			int r = ( imrow - RowOffset ) / RowStep;
			return r < 0 ? 0 : ( r < Nrows ? r : Nrows - 1 );
		}

		public int ImageRowToMatrixRowStrict ( int imrow )
		{
			imrow -= RowOffset;
			int mrow = imrow >= 0 && imrow % RowStep == 0 ? imrow / RowStep : -1;
			return mrow < Nrows ? mrow : -1;
		}

		public int MatrixRowToImageRow ( int mrow )
		{
			return mrow * RowStep + RowOffset;
		}

		public ImageLine GetImageLineAtMatrixRow ( int mrow )
		{
			if ( mrow < 0 || mrow > Nrows )
				throw new PngjException ( "Bad row " + mrow + ". Should be positive and less than "
						+ Nrows );
			ImageLine imline = sampleType == ImageLine.ESampleType.INT ? new ImageLine ( ImgInfo, sampleType,
					SamplesUnpacked, Scanlines [ mrow ], null ) : new ImageLine ( ImgInfo, sampleType,
					SamplesUnpacked, null, ScanlinesB [ mrow ] );
			imline.Rown = MatrixRowToImageRow ( mrow );
			return imline;
		}
	}
}
