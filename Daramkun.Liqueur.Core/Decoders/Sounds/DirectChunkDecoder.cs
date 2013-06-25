using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Medias;

namespace Daramkun.Liqueur.Decoders.Sounds
{
	[FileFormat ( "DIRECTDATA" )]
	public class DirectChunkDecoder : ISoundDecoder
	{
		[Obsolete ( "This method is not support in DirectChunkDecoder", true )]
		public SoundData? Decode ( Stream stream )
		{
			throw new NotImplementedException ();
		}

		public byte [] GetSample ( SoundData soundData )
		{
			object data = soundData.Samples;
			return data as byte [];
		}
	}
}
