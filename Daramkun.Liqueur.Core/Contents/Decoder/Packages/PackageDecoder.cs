using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents.Decoder.Images;
using Daramkun.Liqueur.Contents.FileSystems;
using Daramkun.Liqueur.Exceptions;
using Daramkun.Liqueur.IO.Compression;

namespace Daramkun.Liqueur.Contents.Decoder.Packages
{
	[FileFormat ( "WLNT" )]
	public class PackageDecoder : IDecoder<PackageInfo>
	{
		public PackageInfo Decode ( Stream stream, params object [] args )
		{
			BinaryReader reader = new BinaryReader ( stream );
			if ( Encoding.UTF8.GetString ( reader.ReadBytes ( 4 ), 0, 4 ) != "WLNT" )
				throw new FileFormatMismatchException ();

			DeflateStream deflate = new DeflateStream ( stream, CompressionMode.Decompress );
			reader = new BinaryReader ( deflate );

			PackageInfo packInfo = new PackageInfo ();

			packInfo.PackageName = Encoding.UTF8.GetString ( reader.ReadBytes ( 32 ), 0, 32 ).Trim ( '\0', ' ', '\t', '\n', '　' );

			packInfo.Author = Encoding.UTF8.GetString ( reader.ReadBytes ( 32 ), 0, 32 ).Trim ( '\0', ' ', '\t', '\n', '　' );
			packInfo.Copyright = Encoding.UTF8.GetString ( reader.ReadBytes ( 128 ), 0, 128 ).Trim ( '\0', ' ', '\t', '\n', '　' );
			packInfo.Description = Encoding.UTF8.GetString ( reader.ReadBytes ( 128 ), 0, 128 ).Trim ( '\0', ' ', '\t', '\n', '　' );

			packInfo.PackageID = new Guid ( reader.ReadBytes ( 16 ) );
			packInfo.Version = new Version ( reader.ReadByte (), reader.ReadByte (), reader.ReadByte (), reader.ReadUInt16 () );
			packInfo.ReleaseDate = new DateTime ( reader.ReadInt16 (), reader.ReadByte (), reader.ReadByte () );

			int imageSize = reader.ReadInt32 ();
			if ( imageSize > 0 )
				packInfo.PackageCover = new PngDecoder ().Decode ( new MemoryStream ( reader.ReadBytes ( imageSize ) ) );

			packInfo.IsSubPackage = reader.ReadBoolean ();

			if ( packInfo.IsSubPackage )
			{
				int mainPackCount = reader.ReadByte ();
				if ( mainPackCount > 0 )
				{
					List<Guid> mainGuid = new List<Guid> ();
					for ( int i = 0; i < mainPackCount; i++ )
						mainGuid.Add ( new Guid ( reader.ReadBytes ( 16 ) ) );

					if ( !mainGuid.Contains ( PackageSystem.MainPackage.PackageID ) )
					{
						bool isContains = false;
						foreach ( PackageInfo subpack in PackageSystem.SubPackages )
							if ( mainGuid.Contains ( subpack.PackageID ) )
								isContains = true;
						if ( !isContains )
							throw new SubPackageNotAllowedThisPackageException ();
					}

					packInfo.MainPackageIDs = mainGuid.ToArray ();
				}
			}

			int stringTableSize = reader.ReadInt32 ();
			if ( stringTableSize > 0 )
				packInfo.StringTable = new StringTable ( new MemoryStream ( reader.ReadBytes ( stringTableSize ) ) );

			int resourceTableSize = reader.ReadInt32 ();
			if ( resourceTableSize > 0 )
				packInfo.ResourceTable = new ContentManager ( new ZipFileSystem ( new MemoryStream ( reader.ReadBytes ( resourceTableSize ) ) ) );

			int scriptTableSize = reader.ReadInt32 ();
			if ( scriptTableSize > 0 )
				packInfo.ScriptTable = new ScriptTable ( new MemoryStream ( reader.ReadBytes ( scriptTableSize ) ) );

			return packInfo;
		}
	}
}
