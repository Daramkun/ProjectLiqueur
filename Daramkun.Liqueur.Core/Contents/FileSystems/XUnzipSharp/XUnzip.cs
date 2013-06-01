using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Daramkun.Liqueur.IO.Compression;

namespace Daramkun.Liqueur.Contents.FileSystems.XUnzipSharp
{
	class XUnzip
	{
		Stream inputStream;
		List<XUnzipFileInfo> fileInfoList = new List<XUnzipFileInfo> ();

		enum SIGNATURE : uint
		{
			ERROR = 0x00000000,
			EOF = 0xffffffff,
			LOCAL_FILE_HEADER = 0x04034b50,
			CENTRAL_DIRECTORY_STRUCTURE = 0x02014b50,
			ENDOF_CENTRAL_DIRECTORY_RECORD = 0x06054b50,
		}
		enum ZIP_FILE_ATTRIBUTE
		{
			READONLY = 0x1,
			HIDDEN = 0x2,
			DIRECTORY = 0x10,
			FILE = 0x20,
		}

		public XUnzipFileInfo [] FileInfo { get { return fileInfoList.ToArray (); } }

		public bool Open ( Stream stream )
		{
			return _Open ( stream );
		}

		public void Close ()
		{
			if ( inputStream != null )
				inputStream.Dispose ();
		}

		public bool ExtractTo ( int index, Stream stream )
		{
			return _ExtractTo ( index, stream, 0 );
		}

		private bool _Open ( Stream pInput )
		{
			inputStream = pInput;
			SIGNATURE sig;
			BinaryReader br = new BinaryReader ( inputStream );
			if ( ( sig = ( SIGNATURE ) br.ReadUInt32 () ) != SIGNATURE.LOCAL_FILE_HEADER )
				return false;
			if ( !SearchCentralDirectory ( br ) )
				return false;
			for ( ; ; )
			{
				sig = ( SIGNATURE ) br.ReadUInt32 ();
				if ( sig == SIGNATURE.CENTRAL_DIRECTORY_STRUCTURE )
				{
					if ( !ReadCentralDirectoryStructure ( br ) )
						return false;
				}
				else if ( sig == SIGNATURE.ENDOF_CENTRAL_DIRECTORY_RECORD )
					break;
				else
					return false;
			}

			return true;
		}

		private bool SearchCentralDirectory ( BinaryReader br )
		{
			byte [] buf = new byte [ 0x400 + 4 ];

			uint uMaxBack = 0xffff;
			uint uBackRead;
			uint uSizeFile = ( uint ) inputStream.Length;
			uint uPosFound = 0;

			if ( uMaxBack > uSizeFile )
				uMaxBack = uSizeFile;

			uBackRead = 4;
			while ( uBackRead < uMaxBack )
			{
				uint uReadSize;
				uint uReadPos;
				int i;

				if ( uBackRead + 0x400 > uMaxBack )
					uBackRead = uMaxBack;
				else
					uBackRead += 0x400;
				uReadPos = uSizeFile - uBackRead;

				uReadSize = ( ( 0x400 + 4 < uSizeFile - uReadPos ) ? 0x400 + 4 : uSizeFile - uReadPos );

				if ( ( inputStream.Position = uReadPos ) != uReadPos )
					break;
				if ( br.Read ( buf, 0, ( int ) uReadSize ) != uReadSize )
					break;

				for ( i = ( int ) uReadSize - 3; ( i-- ) > 0; )
				{
					if ( ( buf [ i ] == 0x50 ) && ( buf [ i + 1 ] == 0x4b ) &&
						( buf [ i + 2 ] == 0x05 ) && ( buf [ i + 3 ] == 0x06 ) )
					{
						uPosFound = uReadPos + ( uint ) i;
						break;
					}
				}

				if ( uPosFound != 0 )
					break;
			}

			if ( uPosFound == 0 ) return false;

			inputStream.Seek ( uPosFound, SeekOrigin.Begin );

			SIGNATURE sig;
			if ( ( sig = ( SIGNATURE ) br.ReadUInt32 () ) != SIGNATURE.ENDOF_CENTRAL_DIRECTORY_RECORD )
				return false;

			SEndOfCentralDirectoryRecord rec = new SEndOfCentralDirectoryRecord ();
			rec.numberOfThisDisk = br.ReadInt16 ();
			rec.numberOfTheDiskWithTheStartOfTheCentralDirectory = br.ReadInt16 ();
			rec.centralDirectoryOnThisDisk = br.ReadInt16 ();
			rec.totalNumberOfEntriesInTheCentralDirectoryOnThisDisk = br.ReadInt16 ();
			rec.sizeOfTheCentralDirectory = br.ReadUInt32 ();
			rec.offsetOfStartOfCentralDirectoryWithREspectoTotheStartingDiskNumber = br.ReadUInt32 ();
			rec.zipFileCommentLength = br.ReadInt16 ();

			inputStream.Seek ( rec.offsetOfStartOfCentralDirectoryWithREspectoTotheStartingDiskNumber, SeekOrigin.Begin );

			return true;
		}

		private bool ReadCentralDirectoryStructure ( BinaryReader br )
		{
			string fileName = "";
			XUnzipFileInfo pFileInfo = new XUnzipFileInfo ();
			SCentralDirectoryStructureHead head = new SCentralDirectoryStructureHead ();

			head.versionMadeBy = br.ReadInt16 ();
			head.versionNeededToExtract = br.ReadInt16 ();
			head.generalPurposeBitFlag = new _UGeneralPurposeBitFlag ( br.ReadInt16 () );
			head.compressionMethod = ( XUNZIP_COMPRESSION_METHOD ) br.ReadInt16 ();
			head.dostime = br.ReadUInt32 ();
			head.crc32 = br.ReadUInt32 ();
			head.compressedSize = br.ReadUInt32 ();
			head.uncompressedSize = br.ReadUInt32 ();
			head.fileNameLength = br.ReadInt16 ();
			head.extraFieldLength = br.ReadInt16 ();
			head.fileCommentLength = br.ReadInt16 ();
			head.diskNumberStart = br.ReadInt16 ();
			head.internalFileAttributes = br.ReadInt16 ();
			head.externalFileAttributes = br.ReadUInt32 ();
			head.relativeOffsetOfLocalHeader = br.ReadUInt32 ();

			if ( head.compressionMethod != XUNZIP_COMPRESSION_METHOD.METHOD_STORE &&
				head.compressionMethod != XUNZIP_COMPRESSION_METHOD.METHOD_DEFLATE )
				return false;

			if ( head.generalPurposeBitFlag.bit0 != 0 )
				return false;

			if ( head.fileNameLength != 0 )
			{
				byte [] tmp = br.ReadBytes ( head.fileNameLength );
				fileName = Encoding.UTF8.GetString ( tmp, 0, tmp.Length );
			}

			inputStream.Seek ( head.extraFieldLength, SeekOrigin.Current );

			//inputStream.Seek ( head.extraFieldLength, SeekOrigin.Current );

			inputStream.Seek ( head.fileCommentLength, SeekOrigin.Current );

			if ( ( head.externalFileAttributes & 16 ) != 0 )
				return true;

			pFileInfo.fileName = fileName;
			pFileInfo.compressedSize = ( int ) head.compressedSize;
			pFileInfo.uncompressedSize = ( int ) head.uncompressedSize;
			pFileInfo.crc32 = head.crc32;
			pFileInfo.method = head.compressionMethod;
			pFileInfo.encrypted = ( head.generalPurposeBitFlag.bit0 != 0 ) ? true : false;
			pFileInfo.offsetLocalHeader = ( int ) head.relativeOffsetOfLocalHeader;
			pFileInfo.offsetData = -1;

			FileAddList ( pFileInfo );

			return true;
		}

		private bool _ExtractTo ( int index, Stream output, int outBufRemain )
		{
			XUnzipFileInfo fileInfo = _GetFileInfo ( index );

			BinaryReader br = new BinaryReader ( inputStream );

			if ( fileInfo.offsetData == -1 )
			{
				if ( inputStream.Seek ( fileInfo.offsetLocalHeader, SeekOrigin.Begin ) != fileInfo.offsetLocalHeader )
					throw new Exception ( XUNZIP_ERR.CANT_READ_FILE.ToString () );
				if ( !ReadLocalHeader ( ref fileInfo, br ) )
					return false;
			}

			inputStream.Seek ( fileInfo.offsetData, SeekOrigin.Begin );

			byte [] bufIn;
			int inputRemain;
			int outputRemain;
			int toRead;
			uint crc32 = 0;

			bufIn = new byte [ 32768 ];

			inputRemain = fileInfo.compressedSize;
			outputRemain = fileInfo.uncompressedSize;

			if ( fileInfo.method == XUNZIP_COMPRESSION_METHOD.METHOD_STORE )
			{
				while ( inputRemain > 0 )
				{
					toRead = Math.Min ( Math.Min ( 32768, inputRemain ), outputRemain );

					int temp = br.Read ( bufIn, 0, toRead );
					output.Write ( bufIn, 0, toRead );
					crc32 = ( uint ) SourceDefine.CRC32 ( crc32, bufIn, toRead );

					inputRemain -= toRead;
					outputRemain -= temp;
				}
			}
			else
			{
				DeflateStream stream = new DeflateStream ( inputStream, CompressionMode.Decompress );

				while ( inputRemain > 0 )
				{
					toRead = Math.Min ( Math.Min ( 32768, inputRemain ), outputRemain );

					int temp = stream.Read ( bufIn, 0, outputRemain );
					output.Write ( bufIn, 0, temp );
					crc32 = ( uint ) SourceDefine.CRC32 ( crc32, bufIn, toRead );

					inputRemain -= toRead;
					outputRemain -= temp;
				}
			}

			//if ( crc32 != fileInfo.crc32 )
				//return false;

			if ( outputRemain > 0 )
				return false;

			output.Position = 0;

			return true;
		}

		private XUnzipFileInfo _GetFileInfo ( int index )
		{
			return fileInfoList [ index ];
		}

		private bool ReadLocalHeader ( ref XUnzipFileInfo pFileInfo, BinaryReader br )
		{
			SIGNATURE sig;

			if ( ( sig = ( SIGNATURE ) br.ReadUInt32 () ) != SIGNATURE.LOCAL_FILE_HEADER )
				return false;

			SLocalFileHeader head = new SLocalFileHeader ();

			head.versionNeededToExtract = br.ReadInt16 ();
			head.generalPurposeBitFlag = new _UGeneralPurposeBitFlag ( br.ReadInt16 () );
			head.compressionMethod = ( XUNZIP_COMPRESSION_METHOD ) br.ReadInt16 ();
			head.dostime = br.ReadUInt32 ();
			head.crc32 = br.ReadUInt32 ();
			head.compressedSize = br.ReadUInt32 ();
			head.uncompressedSize = br.ReadUInt32 ();
			head.fileNameLength = br.ReadInt16 ();
			head.extraFieldLength = br.ReadInt16 ();

			inputStream.Seek ( head.fileNameLength, SeekOrigin.Current );

			inputStream.Seek ( head.extraFieldLength, SeekOrigin.Current );

			pFileInfo.offsetData = ( int ) inputStream.Position;

			return true;
		}

		private bool FileAddList ( XUnzipFileInfo pFileInfo )
		{
			fileInfoList.Add ( pFileInfo );
			return true;
		}
	}

	struct SEndOfCentralDirectoryRecord
	{
		internal short numberOfThisDisk;
		internal short numberOfTheDiskWithTheStartOfTheCentralDirectory;
		internal short centralDirectoryOnThisDisk;
		internal short totalNumberOfEntriesInTheCentralDirectoryOnThisDisk;
		internal uint sizeOfTheCentralDirectory;
		internal uint offsetOfStartOfCentralDirectoryWithREspectoTotheStartingDiskNumber;
		internal short zipFileCommentLength;
	}

	[StructLayout ( LayoutKind.Explicit )]
	struct _UGeneralPurposeBitFlag
	{
		[FieldOffset ( 0 )]
		internal short data;
		[FieldOffset ( 0 )]
		internal byte bit0;
		[FieldOffset ( 1 )]
		internal byte bit1;
		[FieldOffset ( 2 )]
		internal byte bit2;
		[FieldOffset ( 3 )]
		internal byte bit3;
		[FieldOffset ( 4 )]
		internal byte bit4;
		[FieldOffset ( 5 )]
		internal byte bit5;
		[FieldOffset ( 6 )]
		internal byte bit6;
		[FieldOffset ( 7 )]
		internal byte bit7;
		[FieldOffset ( 8 )]
		internal byte bit8;
		[FieldOffset ( 9 )]
		internal byte bit9;
		[FieldOffset ( 10 )]
		internal byte bit10;
		[FieldOffset ( 11 )]
		internal byte bit11;

		public _UGeneralPurposeBitFlag ( short d )
		{
			bit0 = bit1 = bit2 = bit3 = bit4 = bit5 = bit6 = bit7 = bit8 = bit9 = bit10 = bit11 = 0;
			data = d;
		}
	}

	struct SLocalFileHeader
	{
		internal short versionNeededToExtract;
		internal _UGeneralPurposeBitFlag generalPurposeBitFlag;
		internal XUNZIP_COMPRESSION_METHOD compressionMethod;
		internal uint dostime;		// lastModFileTime + lastModFileDate*0xffff
		internal uint crc32;
		internal uint compressedSize;
		internal uint uncompressedSize;
		internal short fileNameLength;
		internal short extraFieldLength;
	}

	struct SCentralDirectoryStructureHead
	{
		internal short versionMadeBy;
		internal short versionNeededToExtract;
		internal _UGeneralPurposeBitFlag generalPurposeBitFlag;
		internal XUNZIP_COMPRESSION_METHOD compressionMethod;
		internal uint dostime;
		internal uint crc32;
		internal uint compressedSize;
		internal uint uncompressedSize;
		internal short fileNameLength;
		internal short extraFieldLength;
		internal short fileCommentLength;
		internal short diskNumberStart;
		internal short internalFileAttributes;
		internal uint externalFileAttributes;
		internal uint relativeOffsetOfLocalHeader;
	}

	static class SourceDefine
	{
		private static void DO1 ( ref long crc, byte [] buf, ref int bufIndex )
		{
			uint [] _CRC32 = {0x00000000, 0x77073096, 0xee0e612c, 0x990951ba, 0x076dc419, 0x706af48f,
									0xe963a535, 0x9e6495a3, 0x0edb8832, 0x79dcb8a4, 0xe0d5e91e, 0x97d2d988,
									0x09b64c2b, 0x7eb17cbd, 0xe7b82d07, 0x90bf1d91, 0x1db71064, 0x6ab020f2,
									0xf3b97148, 0x84be41de, 0x1adad47d, 0x6ddde4eb, 0xf4d4b551, 0x83d385c7,
									0x136c9856, 0x646ba8c0, 0xfd62f97a, 0x8a65c9ec, 0x14015c4f, 0x63066cd9,
									0xfa0f3d63, 0x8d080df5, 0x3b6e20c8, 0x4c69105e, 0xd56041e4, 0xa2677172,
									0x3c03e4d1, 0x4b04d447, 0xd20d85fd, 0xa50ab56b, 0x35b5a8fa, 0x42b2986c, 
									0xdbbbc9d6, 0xacbcf940, 0x32d86ce3, 0x45df5c75, 0xdcd60dcf, 0xabd13d59,
									0x26d930ac, 0x51de003a, 0xc8d75180, 0xbfd06116, 0x21b4f4b5, 0x56b3c423,
									0xcfba9599, 0xb8bda50f, 0x2802b89e, 0x5f058808, 0xc60cd9b2, 0xb10be924,
									0x2f6f7c87, 0x58684c11, 0xc1611dab, 0xb6662d3d, 0x76dc4190, 0x01db7106,
									0x98d220bc, 0xefd5102a, 0x71b18589, 0x06b6b51f, 0x9fbfe4a5, 0xe8b8d433, 
									0x7807c9a2, 0x0f00f934, 0x9609a88e, 0xe10e9818, 0x7f6a0dbb, 0x086d3d2d,
									0x91646c97, 0xe6635c01, 0x6b6b51f4, 0x1c6c6162, 0x856530d8, 0xf262004e, 
									0x6c0695ed, 0x1b01a57b, 0x8208f4c1, 0xf50fc457, 0x65b0d9c6, 0x12b7e950,
									0x8bbeb8ea, 0xfcb9887c, 0x62dd1ddf, 0x15da2d49, 0x8cd37cf3, 0xfbd44c65,
									0x4db26158, 0x3ab551ce, 0xa3bc0074, 0xd4bb30e2, 0x4adfa541, 0x3dd895d7,
									0xa4d1c46d, 0xd3d6f4fb, 0x4369e96a, 0x346ed9fc, 0xad678846, 0xda60b8d0, 
									0x44042d73, 0x33031de5, 0xaa0a4c5f, 0xdd0d7cc9, 0x5005713c, 0x270241aa,
									0xbe0b1010, 0xc90c2086, 0x5768b525, 0x206f85b3, 0xb966d409, 0xce61e49f, 
									0x5edef90e, 0x29d9c998, 0xb0d09822, 0xc7d7a8b4, 0x59b33d17, 0x2eb40d81, 
									0xb7bd5c3b, 0xc0ba6cad, 0xedb88320, 0x9abfb3b6, 0x03b6e20c, 0x74b1d29a, 
									0xead54739, 0x9dd277af, 0x04db2615, 0x73dc1683, 0xe3630b12, 0x94643b84, 
									0x0d6d6a3e, 0x7a6a5aa8, 0xe40ecf0b, 0x9309ff9d, 0x0a00ae27, 0x7d079eb1,
									0xf00f9344, 0x8708a3d2, 0x1e01f268, 0x6906c2fe, 0xf762575d, 0x806567cb,
									0x196c3671, 0x6e6b06e7, 0xfed41b76, 0x89d32be0, 0x10da7a5a, 0x67dd4acc, 
									0xf9b9df6f, 0x8ebeeff9, 0x17b7be43, 0x60b08ed5, 0xd6d6a3e8, 0xa1d1937e,
									0x38d8c2c4, 0x4fdff252, 0xd1bb67f1, 0xa6bc5767, 0x3fb506dd, 0x48b2364b, 
									0xd80d2bda, 0xaf0a1b4c, 0x36034af6, 0x41047a60, 0xdf60efc3, 0xa867df55, 
									0x316e8eef, 0x4669be79, 0xcb61b38c, 0xbc66831a, 0x256fd2a0, 0x5268e236,
									0xcc0c7795, 0xbb0b4703, 0x220216b9, 0x5505262f, 0xc5ba3bbe, 0xb2bd0b28,
									0x2bb45a92, 0x5cb36a04, 0xc2d7ffa7, 0xb5d0cf31, 0x2cd99e8b, 0x5bdeae1d,
									0x9b64c2b0, 0xec63f226, 0x756aa39c, 0x026d930a, 0x9c0906a9, 0xeb0e363f,
									0x72076785, 0x05005713, 0x95bf4a82, 0xe2b87a14, 0x7bb12bae, 0x0cb61b38, 
									0x92d28e9b, 0xe5d5be0d, 0x7cdcefb7, 0x0bdbdf21, 0x86d3d2d4, 0xf1d4e242, 
									0x68ddb3f8, 0x1fda836e, 0x81be16cd, 0xf6b9265b, 0x6fb077e1, 0x18b74777, 
									0x88085ae6, 0xff0f6a70, 0x66063bca, 0x11010b5c, 0x8f659eff, 0xf862ae69, 
									0x616bffd3, 0x166ccf45, 0xa00ae278, 0xd70dd2ee, 0x4e048354, 0x3903b3c2,  
									0xa7672661, 0xd06016f7, 0x4969474d, 0x3e6e77db, 0xaed16a4a, 0xd9d65adc, 
									0x40df0b66, 0x37d83bf0, 0xa9bcae53, 0xdebb9ec5, 0x47b2cf7f, 0x30b5ffe9,
									0xbdbdf21c, 0xcabac28a, 0x53b39330, 0x24b4a3a6, 0xbad03605, 0xcdd70693,
									0x54de5729, 0x23d967bf, 0xb3667a2e, 0xc4614ab8, 0x5d681b02, 0x2a6f2b94,
									0xb40bbe37, 0xc30c8ea1, 0x5a05df1b, 0x2d02ef8d };
			crc = _CRC32 [ ( ( int ) crc ^ ( buf [ bufIndex++ ] ) ) & 0xff ] ^ ( crc >> 8 );
		}

		private static void DO8 ( ref long crc, byte [] buf, ref int bufIndex )
		{
			DO1 ( ref crc, buf, ref bufIndex );
			DO1 ( ref crc, buf, ref bufIndex );
			DO1 ( ref crc, buf, ref bufIndex );
			DO1 ( ref crc, buf, ref bufIndex );
			DO1 ( ref crc, buf, ref bufIndex );
			DO1 ( ref crc, buf, ref bufIndex );
			DO1 ( ref crc, buf, ref bufIndex );
			DO1 ( ref crc, buf, ref bufIndex );
		}

		public static long CRC32 ( long crc, byte [] buf, int len )
		{
			if ( buf == null ) return 0;
			crc = crc ^ 0xffffffffL;
			int bufIndex = 0;
			while ( len >= 8 )
			{
				DO8 ( ref crc, buf, ref bufIndex );
				len -= 8;
			}

			if ( len != 0 )
			{
				do
				{
					DO1 ( ref crc, buf, ref bufIndex );
				} while ( --len != 0 );
			}

			return crc ^ 0xffffffffL;
		}
	}

	public struct XUnzipFileInfo
	{
		public string FileName { get { return fileName; } }

		internal string fileName;					// 파일명(CHAR*)
		internal int compressedSize;				// 압축된 크기
		internal int uncompressedSize;				// 압축 안된 크기
		internal uint crc32;
		internal XUNZIP_COMPRESSION_METHOD method;	// 압축 알고리즘
		internal bool encrypted;					// 암호 걸렸남?(BOOL)
		internal int offsetLocalHeader;				// local header의 옵셋
		internal int offsetData;					// 압축 데이타의 옵셋
	}

	enum XUNZIP_ERR
	{
		OK,									// 에러 없음
		CANT_OPEN_FILE,						// 파일 열기 실패
		CANT_READ_FILE,						// 파일 읽기 실패
		BROKEN_FILE,						// 손상된 파일임 (혹은 지원하지 않는 확장 포맷의 zip 파일)
		INVALID_ZIP_FILE,					// ZIP 파일이 아님
		UNSUPPORTED_FORMAT,					// 지원하지 않는 파일 포맷
		ALLOC_FAILED,						// 메모리 alloc 실패
		INVALID_PARAM,						// 잘못된 입력 파라메터
		CANT_WRITE_FILE,					// 파일 출력중 에러 발생
		INFLATE_FAIL,						// inflate 에러 발생
		INVALID_CRC,						// crc 에러 발생
		MALLOC_FAIL,						// 메모리 alloc 실패
		INSUFFICIENT_OUTBUFFER,				// 출력 버퍼가 모자람
	}

	enum XUNZIP_COMPRESSION_METHOD : short
	{
		METHOD_STORE = 0,
		METHOD_DEFLATE = 8,
	}
}
