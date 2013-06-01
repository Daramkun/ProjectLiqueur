using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Medias;

namespace Daramkun.Liqueur.Decoders
{
	/// <summary>
	/// 사운드 디코더 인터페이스
	/// </summary>
	public interface ISoundDecoder : IDecoder<SoundData>
	{
		byte [] GetSample ( SoundData soundData );
	}
}
