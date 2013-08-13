using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents.Decoder.Videos
{
	public interface IVideoDecoder : IDecoder<VideoInfo>
	{
		AudioInfo GetAudio ( VideoInfo videoInfo );
		ImageInfo GetImage ( VideoInfo videoInfo, TimeSpan? timeSpan );
	}
}
