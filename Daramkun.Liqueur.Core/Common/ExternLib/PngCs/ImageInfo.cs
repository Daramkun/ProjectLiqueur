namespace Hjg.Pngcs
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;

	internal class ImageInfo
	{
		private const int MAX_COLS_ROWS_VAL = 400000;

		public readonly int Cols;
		public readonly int Rows;
		public readonly int BitDepth;
		public readonly int Channels;
		public readonly int BitspPixel;
		public readonly int BytesPixel;
		public readonly int BytesPerRow;
		public readonly int SamplesPerRow;

		public readonly int SamplesPerRowPacked;
		public readonly bool Alpha;
		public readonly bool Greyscale;
		public readonly bool Indexed;
		public readonly bool Packed;

		public ImageInfo ( int cols, int rows, int bitdepth, bool alpha )
			: this ( cols, rows, bitdepth, alpha, false, false )
		{
		}

		public ImageInfo ( int cols, int rows, int bitdepth, bool alpha, bool grayscale,
				bool palette )
		{
			this.Cols = cols;
			this.Rows = rows;
			this.Alpha = alpha;
			this.Indexed = palette;
			this.Greyscale = grayscale;
			if ( Greyscale && palette )
				throw new PngjException ( "palette and greyscale are exclusive" );
			this.Channels = ( grayscale || palette ) ? ( ( alpha ) ? 2 : 1 ) : ( ( alpha ) ? 4 : 3 );

			this.BitDepth = bitdepth;
			this.Packed = bitdepth < 8;
			this.BitspPixel = ( Channels * this.BitDepth );
			this.BytesPixel = ( BitspPixel + 7 ) / 8;
			this.BytesPerRow = ( BitspPixel * cols + 7 ) / 8;
			this.SamplesPerRow = Channels * this.Cols;
			this.SamplesPerRowPacked = ( Packed ) ? BytesPerRow : SamplesPerRow;

			switch ( this.BitDepth )
			{
				case 1:
				case 2:
				case 4:
					if ( !( this.Indexed || this.Greyscale ) )
						throw new PngjException ( "only indexed or grayscale can have bitdepth="
								+ this.BitDepth );
					break;
				case 8:
					break;
				case 16:
					if ( this.Indexed )
						throw new PngjException ( "indexed can't have bitdepth=" + this.BitDepth );
					break;
				default:
					throw new PngjException ( "invalid bitdepth=" + this.BitDepth );
			}
			if ( cols < 1 || cols > MAX_COLS_ROWS_VAL )
				throw new PngjException ( "invalid cols=" + cols + " ???" );
			if ( rows < 1 || rows > MAX_COLS_ROWS_VAL )
				throw new PngjException ( "invalid rows=" + rows + " ???" );
		}

		public override String ToString ()
		{
			return "ImageInfo [cols=" + Cols + ", rows=" + Rows + ", bitDepth=" + BitDepth
					+ ", channels=" + Channels + ", bitspPixel=" + BitspPixel + ", bytesPixel="
					+ BytesPixel + ", bytesPerRow=" + BytesPerRow + ", samplesPerRow="
					+ SamplesPerRow + ", samplesPerRowP=" + SamplesPerRowPacked + ", alpha=" + Alpha
					+ ", greyscale=" + Greyscale + ", indexed=" + Indexed + ", packed=" + Packed
					+ "]";
		}

		public override int GetHashCode ()
		{
			int prime = 31;
			int result = 1;
			result = prime * result + ( ( Alpha ) ? 1231 : 1237 );
			result = prime * result + BitDepth;
			result = prime * result + Channels;
			result = prime * result + Cols;
			result = prime * result + ( ( Greyscale ) ? 1231 : 1237 );
			result = prime * result + ( ( Indexed ) ? 1231 : 1237 );
			result = prime * result + Rows;
			return result;
		}

		public override bool Equals ( Object obj )
		{
			if ( ( Object ) this == obj )
				return true;
			if ( obj == null )
				return false;
			if ( ( Object ) GetType () != ( Object ) obj.GetType () )
				return false;
			ImageInfo other = ( ImageInfo ) obj;
			if ( Alpha != other.Alpha )
				return false;
			if ( BitDepth != other.BitDepth )
				return false;
			if ( Channels != other.Channels )
				return false;
			if ( Cols != other.Cols )
				return false;
			if ( Greyscale != other.Greyscale )
				return false;
			if ( Indexed != other.Indexed )
				return false;
			if ( Rows != other.Rows )
				return false;
			return true;
		}
	}
}