using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Medias
{
	/// <summary>
	/// 오디오 채널
	/// </summary>
	public enum AudioChannel
	{
		/// <summary>
		/// 알려지지 않은 채널
		/// </summary>
		Unknown = 0,
		/// <summary>
		/// 1채널
		/// </summary>
		Mono = 1,
		/// <summary>
		/// 2채널
		/// </summary>
		Stereo = 2,
	}
}
