namespace Hjg.Pngcs
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;

	internal class ImageLine
	{
		public ImageInfo ImgInfo { get; private set; }
		public int [] Scanline { get; private set; }
		public byte [] ScanlineB { get; private set; }
		public int Rown { get; set; }

		internal readonly int channels;
		internal readonly int bitDepth;

		public int ElementsPerRow { get; private set; }
		public int maxSampleVal { get; private set; }

		public enum ESampleType
		{
			INT,
			BYTE
		}
		public ESampleType SampleType { get; private set; }

		public bool SamplesUnpacked { get; private set; }

		public FilterType FilterUsed { get; set; }

		public ImageLine ( ImageInfo imgInfo )
			: this ( imgInfo, ESampleType.INT, false )
		{
		}

		public ImageLine ( ImageInfo imgInfo, ESampleType stype )
			: this ( imgInfo, stype, false )
		{
		}

		public ImageLine ( ImageInfo imgInfo, ESampleType stype, bool unpackedMode )
			: this ( imgInfo, stype, unpackedMode, null, null )
		{
		}

		internal ImageLine ( ImageInfo imgInfo, ESampleType stype, bool unpackedMode, int [] sci, byte [] scb )
		{
			this.ImgInfo = imgInfo;
			channels = imgInfo.Channels;
			this.bitDepth = imgInfo.BitDepth;
			this.FilterUsed = FilterType.FILTER_UNKNOWN;
			this.SampleType = stype;
			this.SamplesUnpacked = unpackedMode || !imgInfo.Packed;
			ElementsPerRow = this.SamplesUnpacked ? imgInfo.SamplesPerRow
				: imgInfo.SamplesPerRowPacked;
			if ( stype == ESampleType.INT )
			{
				Scanline = sci != null ? sci : new int [ ElementsPerRow ];
				ScanlineB = null;
				maxSampleVal = bitDepth == 16 ? 0xFFFF : GetMaskForPackedFormatsLs ( bitDepth );
			}
			else if ( stype == ESampleType.BYTE )
			{
				ScanlineB = scb != null ? scb : new byte [ ElementsPerRow ];
				Scanline = null;
				maxSampleVal = bitDepth == 16 ? 0xFF : GetMaskForPackedFormatsLs ( bitDepth );
			}
			else
				throw new PngjExceptionInternal ( "bad ImageLine initialization" );
			this.Rown = -1;
		}

		static internal void unpackInplaceInt ( ImageInfo iminfo, int [] src, int [] dst,
			bool Scale )
		{
			int bitDepth = iminfo.BitDepth;
			if ( bitDepth >= 8 )
				return;
			int mask0 = GetMaskForPackedFormatsLs ( bitDepth );
			int scalefactor = 8 - bitDepth;
			int offset0 = 8 * iminfo.SamplesPerRowPacked - bitDepth * iminfo.SamplesPerRow;
			int mask, offset, v;
			if ( offset0 != 8 )
			{
				mask = mask0 << offset0;
				offset = offset0;
			}
			else
			{
				mask = mask0;
				offset = 0;
			}
			for ( int j = iminfo.SamplesPerRow - 1, i = iminfo.SamplesPerRowPacked - 1; j >= 0; j-- )
			{
				v = ( src [ i ] & mask ) >> offset;
				if ( Scale )
					v <<= scalefactor;
				dst [ j ] = v;
				mask <<= bitDepth;
				offset += bitDepth;
				if ( offset == 8 )
				{
					mask = mask0;
					offset = 0;
					i--;
				}
			}
		}

		static internal void packInplaceInt ( ImageInfo iminfo, int [] src, int [] dst,
			bool scaled )
		{
			int bitDepth = iminfo.BitDepth;
			if ( bitDepth >= 8 )
				return;
			int mask0 = GetMaskForPackedFormatsLs ( bitDepth );
			int scalefactor = 8 - bitDepth;
			int offset0 = 8 - bitDepth;
			int v, v0;
			int offset = 8 - bitDepth;
			v0 = src [ 0 ];
			dst [ 0 ] = 0;
			if ( scaled )
				v0 >>= scalefactor;
			v0 = ( ( v0 & mask0 ) << offset );
			for ( int i = 0, j = 0; j < iminfo.SamplesPerRow; j++ )
			{
				v = src [ j ];
				if ( scaled )
					v >>= scalefactor;
				dst [ i ] |= ( ( v & mask0 ) << offset );
				offset -= bitDepth;
				if ( offset < 0 )
				{
					offset = offset0;
					i++;
					dst [ i ] = 0;
				}
			}
			dst [ 0 ] |= v0;
		}

		static internal void unpackInplaceByte ( ImageInfo iminfo, byte [] src,
			 byte [] dst, bool scale )
		{
			int bitDepth = iminfo.BitDepth;
			if ( bitDepth >= 8 )
				return;
			int mask0 = GetMaskForPackedFormatsLs ( bitDepth );
			int scalefactor = 8 - bitDepth;
			int offset0 = 8 * iminfo.SamplesPerRowPacked - bitDepth * iminfo.SamplesPerRow;
			int mask, offset, v;
			if ( offset0 != 8 )
			{
				mask = mask0 << offset0;
				offset = offset0;
			}
			else
			{
				mask = mask0;
				offset = 0;
			}
			for ( int j = iminfo.SamplesPerRow - 1, i = iminfo.SamplesPerRowPacked - 1; j >= 0; j-- )
			{
				v = ( src [ i ] & mask ) >> offset;
				if ( scale )
					v <<= scalefactor;
				dst [ j ] = ( byte ) v;
				mask <<= bitDepth;
				offset += bitDepth;
				if ( offset == 8 )
				{
					mask = mask0;
					offset = 0;
					i--;
				}
			}
		}

		static internal void packInplaceByte ( ImageInfo iminfo, byte [] src, byte [] dst,
				 bool scaled )
		{
			int bitDepth = iminfo.BitDepth;
			if ( bitDepth >= 8 )
				return;
			byte mask0 = ( byte ) GetMaskForPackedFormatsLs ( bitDepth );
			byte scalefactor = ( byte ) ( 8 - bitDepth );
			byte offset0 = ( byte ) ( 8 - bitDepth );
			byte v, v0;
			int offset = 8 - bitDepth;
			v0 = src [ 0 ];
			dst [ 0 ] = 0;
			if ( scaled )
				v0 >>= scalefactor;
			v0 = ( byte ) ( ( v0 & mask0 ) << offset );
			for ( int i = 0, j = 0; j < iminfo.SamplesPerRow; j++ )
			{
				v = src [ j ];
				if ( scaled )
					v >>= scalefactor;
				dst [ i ] |= ( byte ) ( ( v & mask0 ) << offset );
				offset -= bitDepth;
				if ( offset < 0 )
				{
					offset = offset0;
					i++;
					dst [ i ] = 0;
				}
			}
			dst [ 0 ] |= v0;
		}

		internal void SetScanLine ( int [] b )
		{
			System.Array.Copy ( ( Array ) ( b ), 0, ( Array ) ( Scanline ), 0, Scanline.Length );
		}

		internal int [] GetScanLineCopy ( int [] b )
		{
			if ( b == null || b.Length < Scanline.Length )
				b = new int [ Scanline.Length ];
			System.Array.Copy ( ( Array ) ( Scanline ), 0, ( Array ) ( b ), 0, Scanline.Length );
			return b;
		}

		public ImageLine unpackToNewImageLine ()
		{
			ImageLine newline = new ImageLine ( ImgInfo, SampleType, true );
			if ( SampleType == ESampleType.INT )
				unpackInplaceInt ( ImgInfo, Scanline, newline.Scanline, false );
			else
				unpackInplaceByte ( ImgInfo, ScanlineB, newline.ScanlineB, false );
			return newline;
		}

		public ImageLine packToNewImageLine ()
		{
			ImageLine newline = new ImageLine ( ImgInfo, SampleType, false );
			if ( SampleType == ESampleType.INT )
				packInplaceInt ( ImgInfo, Scanline, newline.Scanline, false );
			else
				packInplaceByte ( ImgInfo, ScanlineB, newline.ScanlineB, false );
			return newline;
		}

		public int [] GetScanlineInt ()
		{
			return Scanline;
		}

		public byte [] GetScanlineByte ()
		{
			return ScanlineB;
		}

		public bool IsInt ()
		{
			return SampleType == ESampleType.INT;
		}

		public bool IsByte ()
		{
			return SampleType == ESampleType.BYTE;
		}


		public override String ToString ()
		{
			return "row=" + Rown + " cols=" + ImgInfo.Cols + " bpc=" + ImgInfo.BitDepth
					+ " size=" + Scanline.Length;
		}

		internal static int GetMaskForPackedFormats ( int bitDepth )
		{
			if ( bitDepth == 4 ) return 0xf0;
			else if ( bitDepth == 2 ) return 0xc0;
			else if ( bitDepth == 1 ) return 0x80;
			else return 0xff;
		}

		internal static int GetMaskForPackedFormatsLs ( int bitDepth )
		{
			if ( bitDepth == 4 ) return 0x0f;
			else if ( bitDepth == 2 ) return 0x03;
			else if ( bitDepth == 1 ) return 0x01;
			else return 0xff;
		}
	}
}
