using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.Math;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Decoders.Images
{
	[FileFormat ( "BMP", "DIB" )]
	public class DefaultBitmapDecoder : IImageDecoder
	{
		private bool LoadBitmapFileHeader ( BinaryReader reader, ref ImageData imageData, out BITMAPFILEHEADER fileHeader )
		{
			fileHeader = new BITMAPFILEHEADER ();
			fileHeader.bfType = reader.ReadUInt16 ();
			if ( fileHeader.bfType != 0x4D42 )
				return false;
			fileHeader.bfSize = reader.ReadUInt32 ();
			fileHeader.bfReserved1 = reader.ReadUInt16 ();
			fileHeader.bfReserved2 = reader.ReadUInt16 ();
			// 예약된 공간이 0이 아니면 종료
			if ( fileHeader.bfReserved1 != 0 || fileHeader.bfReserved2 != 0 )
				return false;
			fileHeader.bfOffBits = reader.ReadUInt32 ();

			return true;
		}

		private bool LoadBitmapInfoHeader ( BinaryReader reader, ref ImageData imageData, out BITMAPINFOHEADER infoHeader, out int padding )
		{
			infoHeader = new BITMAPINFOHEADER ();

			padding = 0;

			infoHeader.biSize = reader.ReadUInt32 ();
			// 정보 구조체 크기가 40이 아니면 V3 비트맵이 아니므로 종료
			if ( infoHeader.biSize != 40 )
				return false;

			infoHeader.biWidth = reader.ReadInt32 ();
			infoHeader.biHeight = reader.ReadInt32 ();
			infoHeader.biPlanes = reader.ReadUInt16 ();
			infoHeader.biBitCount = reader.ReadUInt16 ();
			infoHeader.biCompression = reader.ReadUInt32 ();
			infoHeader.biSizeImage = reader.ReadUInt32 ();
			infoHeader.biXPelsPerMeter = reader.ReadInt32 ();
			infoHeader.biYPelsPerMeter = reader.ReadInt32 ();
			infoHeader.biClrUsed = reader.ReadUInt32 ();
			infoHeader.biClrImportant = reader.ReadUInt32 ();

			// 1장이 아니면 BMP가 아니므로 종료
			if ( infoHeader.biPlanes != 1 )
				return false;
			// 24비트 또는 32비트 비트맵이 아니면 종료
			if ( !( infoHeader.biBitCount == 24 || infoHeader.biBitCount == 32 ) )
				return false;
			// 압축된 데이터일 경우 종료
			if ( infoHeader.biCompression != 0 )
				return false;

			imageData.Width = infoHeader.biWidth;
			imageData.Height = infoHeader.biHeight;

			int rowWidth = infoHeader.biWidth * 24 / 8;
			padding = 0;
			while ( rowWidth % 4 != 0 )
			{
				rowWidth++;
				padding++;
			}

			if ( infoHeader.biSizeImage == 0 )
				infoHeader.biSizeImage = ( uint ) ( rowWidth * infoHeader.biHeight * ( ( infoHeader.biBitCount == 24 ) ? 3 : 4 ) );

			return true;
		}

		public ImageData? Decode ( Stream stream )
		{
			BinaryReader reader = new BinaryReader ( stream );

			ImageData imageData = new ImageData ();
			BITMAPFILEHEADER fileHeader;
			BITMAPINFOHEADER infoHeader;
			int padding;

			if ( !LoadBitmapFileHeader ( reader, ref imageData, out fileHeader ) )
				throw new FileFormatMismatchException ( "File Header is not bitmap header." );

			if ( !LoadBitmapInfoHeader ( reader, ref imageData, out infoHeader, out padding ) )
				throw new FileFormatMismatchException ( "This file is not 24bpp or 32bpp Bitmap File or other reason." );

			stream.Seek ( fileHeader.bfOffBits, SeekOrigin.Begin );
			imageData.Data = new object [] { padding, infoHeader.biBitCount, reader.ReadBytes ( ( int ) infoHeader.biSizeImage ) };

			return imageData;
		}

		public Color [] GetPixels ( ImageData imageData, Color colorKey )
		{
			object [] imageDataData = ( ( object ) imageData.Data ) as object [];
			int padding = ( int ) imageDataData [ 0 ];
			ushort bpp = ( ushort ) imageDataData [ 1 ];
			byte [] pixels = imageDataData [ 2 ] as byte [];

			Color [] convPixels = new Color [ imageData.Width * imageData.Height ];
			int index = 0;
			if ( bpp == 24 )
			{
				for ( int i = 0; i < imageData.Height; i++ )
				{
					for ( int j = 0; j < imageData.Width; j++ )
					{
						index = ( ( imageData.Height - i - 1 ) * ( imageData.Width * 3 + padding ) + j * 3 );
						Color color = new Color ( pixels [ index + 2 ], pixels [ index + 1 ], pixels [ index + 0 ] );
						convPixels [ ( i * imageData.Width ) + j ] = ( color == colorKey ) ? Color.Transparent : color;
					}
				}
			}
			else if ( bpp == 32 )
			{
				/*
				for ( int i = 0; i < imageData.Height; i++ )
				{
					for ( int j = 0; j < imageData.Width; j++ )
					{
						index = ( ( imageData.Height - i - 1 ) + imageData.Width ) * 4;
						Color color = new Color ( pixels [ index + 2 ], pixels [ index + 1 ], pixels [ index + 0 ], pixels [ index + 3 ] );
						convPixels [ ( i * imageData.Width ) + j ] = ( color == colorKey ) ? Color.Transparent : color;
					}
				}*/
				for ( int i = 0; i < imageData.Height; i++ )
				{
					for ( int j = 0; j < imageData.Width; j++ )
					{
						index = ( ( imageData.Height - i - 1 ) * ( imageData.Width * 4 ) + j * 4 );
						Color color = new Color ( pixels [ index + 2 ], pixels [ index + 1 ], pixels [ index + 0 ], pixels [ index + 3 ] );
						convPixels [ ( i * imageData.Width ) + j ] = ( color == colorKey ) ? Color.Transparent : color;
					}
				}
			}
			imageData.ImageStream.Dispose ();
			return convPixels;
		}

		[StructLayout ( LayoutKind.Sequential )]
		public struct BITMAPFILEHEADER
		{
			public ushort bfType;
			public uint bfSize;
			public ushort bfReserved1;
			public ushort bfReserved2;
			public uint bfOffBits;
		}

		[StructLayout ( LayoutKind.Sequential )]
		public struct BITMAPINFOHEADER
		{
			public uint biSize;
			public int biWidth;
			public int biHeight;
			public ushort biPlanes;
			public ushort biBitCount;
			public uint biCompression;
			public uint biSizeImage;
			public int biXPelsPerMeter;
			public int biYPelsPerMeter;
			public uint biClrUsed;
			public uint biClrImportant;
		}
	}
}