namespace Hjg.Pngcs.Chunks
{
	using Hjg.Pngcs;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.CompilerServices;

	internal class PngChunkICCP : PngChunkSingle
	{
		public const String ID = ChunkHelper.iCCP;

		private String profileName;

		private byte [] compressedProfile;

		public PngChunkICCP ( ImageInfo info )
			: base ( ID, info )
		{
		}

		public override ChunkOrderingConstraint GetOrderingConstraint ()
		{
			return ChunkOrderingConstraint.BEFORE_PLTE_AND_IDAT;
		}

		public override ChunkRaw CreateRawChunk ()
		{
			ChunkRaw c = createEmptyChunk ( profileName.Length + compressedProfile.Length + 2, true );
			System.Array.Copy ( ( Array ) ( Hjg.Pngcs.Chunks.ChunkHelper.ToBytes ( profileName ) ), 0, ( Array ) ( c.Data ), 0, profileName.Length );
			c.Data [ profileName.Length ] = 0;
			c.Data [ profileName.Length + 1 ] = 0;
			System.Array.Copy ( ( Array ) ( compressedProfile ), 0, ( Array ) ( c.Data ), profileName.Length + 2, compressedProfile.Length );
			return c;
		}

		public override void ParseFromRaw ( ChunkRaw chunk )
		{
			int pos0 = Hjg.Pngcs.Chunks.ChunkHelper.PosNullByte ( chunk.Data );
			profileName = Hjg.Pngcs.PngHelperInternal.charsetLatin1.GetString ( chunk.Data, 0, pos0 );
			int comp = ( chunk.Data [ pos0 + 1 ] & 0xff );
			if ( comp != 0 )
				throw new Exception ( "bad compression for ChunkTypeICCP" );
			int compdatasize = chunk.Data.Length - ( pos0 + 2 );
			compressedProfile = new byte [ compdatasize ];
			System.Array.Copy ( ( Array ) ( chunk.Data ), pos0 + 2, ( Array ) ( compressedProfile ), 0, compdatasize );
		}

		public override void CloneDataFromRead ( PngChunk other )
		{
			PngChunkICCP otherx = ( PngChunkICCP ) other;
			profileName = otherx.profileName;
			compressedProfile = new byte [ otherx.compressedProfile.Length ];
			System.Array.Copy ( otherx.compressedProfile, compressedProfile, compressedProfile.Length );
		}

		public void SetProfileNameAndContent ( String name, String profile )
		{
			SetProfileNameAndContent ( name, ChunkHelper.ToBytes ( profileName ) );
		}

		public void SetProfileNameAndContent ( String name, byte [] profile )
		{
			profileName = name;
			compressedProfile = ChunkHelper.compressBytes ( profile, true );
		}

		public String GetProfileName ()
		{
			return profileName;
		}

		public byte [] GetProfile ()
		{
			return ChunkHelper.compressBytes ( compressedProfile, false );
		}

		public String GetProfileAsString ()
		{
			return ChunkHelper.ToString ( GetProfile () );
		}

	}
}
