using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Daramkun.Liqueur.Data.Checksums;
using Daramkun.Liqueur.IO.Compression;

namespace Daramkun.Liqueur.Contents.FileSystems
{
	/// <summary>
	/// ZIP File System class
	/// </summary>
	public class ZipFileSystem : IFileSystem, IDirectorableFileSystem, IDisposable
	{
		XUnzip xUnzip;
		string [] filenameCache;
		string [] dirnameCache;
		Dictionary<string, int> indexInfo;

		/// <summary>
		/// Constructor of Zip File System
		/// </summary>
		/// <param name="stream">ZIP stream</param>
		public ZipFileSystem ( Stream stream )
		{
			xUnzip = new XUnzip ();
			xUnzip.Open ( stream );

			indexInfo = new Dictionary<string, int> ();

			List<string> files = new List<string> ();
			List<string> dirs = new List<string> ();
			int index = 0;
			foreach ( XUnzipFileInfo info in xUnzip.FileInfo )
			{
				indexInfo.Add ( info.FileName, index++ );
				files.Add ( info.FileName );
				string [] temp = info.FileName.Split ( '/' );
				if ( temp.Length == 1 )
					temp = info.FileName.Split ( '\\' );
				temp [ temp.Length - 1 ] = "";
				string path = string.Join ( "/", temp );
				if ( dirs.Contains ( path ) )
					continue;
				dirs.Add ( path );
			}
			filenameCache = files.ToArray ();
			dirnameCache = dirs.ToArray ();
		}

		~ZipFileSystem ()
		{
			Dispose ( false );
		}

		/// <summary>
		/// Disposing
		/// </summary>
		/// <param name="isDisposing">Is Disposing</param>
		protected virtual void Dispose ( bool isDisposing )
		{
			if ( isDisposing )
			{
				xUnzip.Close ();
				xUnzip = null;
			}
		}

		/// <summary>
		/// Dispose Zip File System
		/// </summary>
		public void Dispose ()
		{
			Dispose ( true );
			GC.SuppressFinalize ( this );
		}

		/// <summary>
		/// Is File exist?
		/// </summary>
		/// <param name="filename">Filename with path</param>
		/// <returns>True when file is exist, False when elsecase</returns>
		public bool IsFileExist ( string filename )
		{
			foreach ( XUnzipFileInfo info in xUnzip.FileInfo )
				if ( info.FileName == filename )
					return true;
			return false;
		}

		/// <summary>
		/// Is Directory exist?
		/// </summary>
		/// <param name="filename">Directory name with path</param>
		/// <returns>True when directory is exist, False when elsecase</returns>
		public bool IsDirectoryExist ( string directoryname )
		{
			foreach ( string dir in Directories )
				if ( dir == directoryname ) return true;
			return false;
		}

		/// <summary>
		/// Open file stream
		/// </summary>
		/// <param name="filename">Filename with path</param>
		/// <returns>Opened stream</returns>
		public Stream OpenFile ( string filename )
		{
			lock ( xUnzip )
			{
				if ( !indexInfo.ContainsKey ( filename ) ) return null;

				MemoryStream stream = new MemoryStream ();
				if ( !xUnzip.ExtractTo ( indexInfo [ filename ], stream ) )
					return null;
				return stream;
			}
		}

		/// <summary>
		/// Filenames with path
		/// </summary>
		public string [] Files { get { return filenameCache; } }
		/// <summary>
		/// Directory names
		/// </summary>
		public string [] Directories { get { return dirnameCache; } }
	}

	#region XUnzip
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
				CRC32 crcGen = new CRC32 ();
				while ( inputRemain > 0 )
				{
					toRead = System.Math.Min ( System.Math.Min ( 32768, inputRemain ), outputRemain );

					int temp = br.Read ( bufIn, 0, toRead );
					output.Write ( bufIn, 0, toRead );
					crcGen.Update ( bufIn, 0, toRead );
					crc32 = crcGen.Result;

					inputRemain -= toRead;
					outputRemain -= temp;
				}
			}
			else
			{
				DeflateStream stream = new DeflateStream ( inputStream, CompressionMode.Decompress );

				CRC32 crcGen = new CRC32 ();
				while ( inputRemain > 0 )
				{
					toRead = System.Math.Min ( System.Math.Min ( 32768, inputRemain ), outputRemain );

					int temp = stream.Read ( bufIn, 0, outputRemain );
					output.Write ( bufIn, 0, temp );
					crcGen.Update ( bufIn, 0, toRead );
					crc32 = crcGen.Result;

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
	#endregion
}
