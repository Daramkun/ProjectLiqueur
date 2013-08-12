using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Contents.Decoder.Images
{
	/// <summary>
	/// Bitmap image decoder
	/// </summary>
	[FileFormat ( "BMP", "DIB" )]
	public class BitmapDecoder : IImageDecoder
	{
		private bool LoadBitmapFileHeader ( BinaryReader reader, ref ImageInfo imageData,
			out Utilities.BITMAPFILEHEADER fileHeader )
		{
			fileHeader = new Utilities.BITMAPFILEHEADER ();
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
		private bool LoadBitmapInfoHeader ( BinaryReader reader, ref ImageInfo imageData,
			out Utilities.BITMAPINFOHEADER infoHeader, out int padding )
		{
			infoHeader = new Utilities.BITMAPINFOHEADER ();

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

		/// <summary>
		/// BMP image decode
		/// </summary>
		/// <param name="stream">BMP file</param>
		/// <param name="args">argument, don't set this</param>
		/// <returns>Image information and pixel data</returns>
		public ImageInfo Decode ( Stream stream, params object [] args )
		{
			BinaryReader reader = new BinaryReader ( stream );

			ImageInfo imageInfo = new ImageInfo ();
			Utilities.BITMAPFILEHEADER fileHeader;
			Utilities.BITMAPINFOHEADER infoHeader;
			int padding;

			if ( !LoadBitmapFileHeader ( reader, ref imageInfo, out fileHeader ) )
				throw new FileFormatMismatchException ( "File Header is not bitmap header." );

			if ( !LoadBitmapInfoHeader ( reader, ref imageInfo, out infoHeader, out padding ) )
				throw new FileFormatMismatchException ( "This file is not 24bpp or 32bpp Bitmap File or other reason." );

			stream.Seek ( fileHeader.bfOffBits, SeekOrigin.Begin );
			imageInfo.Data = new object [] { padding, infoHeader.biBitCount, reader.ReadBytes ( ( int ) infoHeader.biSizeImage ) };

			imageInfo.ImageStream = stream;
			imageInfo.ImageDecoder = this;

			return imageInfo;
		}

		/// <summary>
		/// Get BMP pixels
		/// </summary>
		/// <param name="imageInfo">Image information</param>
		/// <param name="colorKey">Color key (if you need)</param>
		/// <returns>Image pixels</returns>
		public Color [] GetPixel ( ImageInfo imageInfo, Color? colorKey )
		{
			object [] imageDataData = ( ( object ) imageInfo.Data ) as object [];
			int padding = ( int ) imageDataData [ 0 ];
			ushort bpp = ( ushort ) imageDataData [ 1 ];
			byte [] pixels = imageDataData [ 2 ] as byte [];

			Color [] convPixels = new Color [ imageInfo.Width * imageInfo.Height ];
			int index = 0;
			if ( bpp == 24 )
			{
				for ( int i = 0; i < imageInfo.Height; i++ )
				{
					for ( int j = 0; j < imageInfo.Width; j++ )
					{
						index = ( ( imageInfo.Height - i - 1 ) * ( imageInfo.Width * 3 + padding ) + j * 3 );
						Color color = new Color ( pixels [ index + 2 ], pixels [ index + 1 ], pixels [ index + 0 ] );
						convPixels [ ( i * imageInfo.Width ) + j ] = ( color == colorKey ) ? Color.Transparent : color;
					}
				}
			}
			else if ( bpp == 32 )
			{
				for ( int i = 0; i < imageInfo.Height; i++ )
				{
					for ( int j = 0; j < imageInfo.Width; j++ )
					{
						index = ( ( imageInfo.Height - i - 1 ) * ( imageInfo.Width * 4 ) + j * 4 );
						Color color = new Color ( pixels [ index + 2 ], pixels [ index + 1 ], pixels [ index + 0 ], pixels [ index + 3 ] );
						convPixels [ ( i * imageInfo.Width ) + j ] = ( color == colorKey ) ? Color.Transparent : color;
					}
				}
			}
			imageInfo.ImageStream.Dispose ();
			return convPixels;
		}

		/// <summary>
		/// Decoder information string
		/// </summary>
		/// <returns></returns>
		public override string ToString ()
		{
			return "Default 24bit/32bit Bitmap Decoder";
		}
	}
}
