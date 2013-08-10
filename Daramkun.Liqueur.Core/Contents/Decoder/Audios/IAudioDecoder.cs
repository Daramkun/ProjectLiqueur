using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents.Decoder.Audios
{
	public interface IAudioDecoder : IDecoder<AudioInfo>
	{
		byte [] GetSample ( AudioInfo audioInfo, TimeSpan? timeSpan );
	}
}
