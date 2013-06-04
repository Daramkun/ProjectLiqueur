using Hjg.Pngcs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Hjg.Pngcs
{
	class PngDeinterlacer
	{
		private readonly ImageInfo imi;
		private int pass;
		private int rows, cols, dY, dX, oY, oX, oXsamples, dXsamples;
		private int currRowSubimg = -1;
		private int currRowReal = -1;
		private readonly int packedValsPerPixel;
		private readonly int packedMask;
		private readonly int packedShift;
		private int [] [] imageInt;
		private byte [] [] imageByte;

		internal PngDeinterlacer ( ImageInfo iminfo )
		{
			this.imi = iminfo;
			pass = 0;
			if ( imi.Packed )
			{
				packedValsPerPixel = 8 / imi.BitDepth;
				packedShift = imi.BitDepth;
				if ( imi.BitDepth == 1 )
					packedMask = 0x80;
				else if ( imi.BitDepth == 2 )
					packedMask = 0xc0;
				else
					packedMask = 0xf0;
			}
			else
			{
				packedMask = packedShift = packedValsPerPixel = 1;// dont care
			}
			setPass ( 1 );
			setRow ( 0 );
		}

		internal void setRow ( int n )
		{
			currRowSubimg = n;
			currRowReal = n * dY + oY;
			if ( currRowReal < 0 || currRowReal >= imi.Rows )
				throw new PngjExceptionInternal ( "bad row - this should not happen" );
		}

		internal void setPass ( int p )
		{
			if ( this.pass == p )
				return;
			pass = p;
			switch ( pass )
			{
				case 1:
					dY = dX = 8;
					oX = oY = 0;
					break;
				case 2:
					dY = dX = 8;
					oX = 4;
					oY = 0;
					break;
				case 3:
					dX = 4;
					dY = 8;
					oX = 0;
					oY = 4;
					break;
				case 4:
					dX = dY = 4;
					oX = 2;
					oY = 0;
					break;
				case 5:
					dX = 2;
					dY = 4;
					oX = 0;
					oY = 2;
					break;
				case 6:
					dX = dY = 2;
					oX = 1;
					oY = 0;
					break;
				case 7:
					dX = 1;
					dY = 2;
					oX = 0;
					oY = 1;
					break;
				default:
					throw new PngjExceptionInternal ( "bad interlace pass" + pass );
			}

			rows = ( imi.Rows - oY ) / dY + 1;
			if ( ( rows - 1 ) * dY + oY >= imi.Rows )
				rows--;
			cols = ( imi.Cols - oX ) / dX + 1;
			if ( ( cols - 1 ) * dX + oX >= imi.Cols )
				cols--;
			if ( cols == 0 )
				rows = 0;
			dXsamples = dX * imi.Channels;
			oXsamples = oX * imi.Channels;
		}

		internal void deinterlaceInt ( int [] src, int [] dst, bool readInPackedFormat )
		{
			if ( !( imi.Packed && readInPackedFormat ) )
				for ( int i = 0, j = oXsamples; i < cols * imi.Channels; i += imi.Channels, j += dXsamples )
					for ( int k = 0; k < imi.Channels; k++ )
						dst [ j + k ] = src [ i + k ];
			else
				deinterlaceIntPacked ( src, dst );
		}

		private void deinterlaceIntPacked ( int [] src, int [] dst )
		{
			int spos, smod, smask;
			int tpos, tmod, p, d;
			spos = 0;
			smask = packedMask;
			smod = -1;

			for ( int i = 0, j = oX; i < cols; i++, j += dX )
			{
				spos = i / packedValsPerPixel;
				smod += 1;
				if ( smod >= packedValsPerPixel )
					smod = 0;
				smask >>= packedShift;
				if ( smod == 0 )
					smask = packedMask;
				tpos = j / packedValsPerPixel;
				tmod = j % packedValsPerPixel;
				p = src [ spos ] & smask;
				d = tmod - smod;
				if ( d > 0 )
					p >>= ( d * packedShift );
				else if ( d < 0 )
					p <<= ( ( -d ) * packedShift );
				dst [ tpos ] |= p;
			}
		}

		internal void deinterlaceByte ( byte [] src, byte [] dst, bool readInPackedFormat )
		{
			if ( !( imi.Packed && readInPackedFormat ) )
				for ( int i = 0, j = oXsamples; i < cols * imi.Channels; i += imi.Channels, j += dXsamples )
					for ( int k = 0; k < imi.Channels; k++ )
						dst [ j + k ] = src [ i + k ];
			else
				deinterlacePackedByte ( src, dst );
		}

		private void deinterlacePackedByte ( byte [] src, byte [] dst )
		{
			int spos, smod, smask;
			int tpos, tmod, p, d;
			spos = 0;
			smask = packedMask;
			smod = -1;
			for ( int i = 0, j = oX; i < cols; i++, j += dX )
			{
				spos = i / packedValsPerPixel;
				smod += 1;
				if ( smod >= packedValsPerPixel )
					smod = 0;
				smask >>= packedShift;
				if ( smod == 0 )
					smask = packedMask;
				tpos = j / packedValsPerPixel;
				tmod = j % packedValsPerPixel;
				p = src [ spos ] & smask;
				d = tmod - smod;
				if ( d > 0 )
					p >>= ( d * packedShift );
				else if ( d < 0 )
					p <<= ( ( -d ) * packedShift );
				dst [ tpos ] |= ( byte ) p;
			}
		}

		internal bool isAtLastRow ()
		{
			return pass == 7 && currRowSubimg == rows - 1;
		}

		internal int getCurrRowSubimg ()
		{
			return currRowSubimg;
		}

		internal int getCurrRowReal ()
		{
			return currRowReal;
		}

		internal int getPass ()
		{
			return pass;
		}

		internal int getRows ()
		{
			return rows;
		}

		internal int getCols ()
		{
			return cols;
		}

		internal int getPixelsToRead ()
		{
			return getCols ();
		}

		internal int [] [] getImageInt ()
		{
			return imageInt;
		}

		internal void setImageInt ( int [] [] imageInt )
		{
			this.imageInt = imageInt;
		}

		internal byte [] [] getImageByte ()
		{
			return imageByte;
		}

		internal void setImageByte ( byte [] [] imageByte )
		{
			this.imageByte = imageByte;
		}

	}
}
