using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Contents.Decoder.Audios
{
	/// <summary>
	/// Audio decoder interface
	/// </summary>
	public interface IAudioDecoder : IDecoder<AudioInfo>
	{
		/// <summary>
		/// Get audio samples
		/// </summary>
		/// <param name="audioInfo">Audio information</param>
		/// <param name="timeSpan">Audio position (if you need)</param>
		/// <returns>Audio sample</returns>
		byte [] GetSample ( AudioInfo audioInfo, TimeSpan? timeSpan );
	}
}
