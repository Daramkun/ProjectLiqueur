using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Audio
{
	/// <summary>
	/// Audio channel enumeration
	/// </summary>
	public enum AudioChannel
	{
		/// <summary>
		/// Unknown (error)
		/// </summary>
		Unknown,
		/// <summary>
		/// Mono (1 channel)
		/// </summary>
		Mono,
		/// <summary>
		/// Stereo (2 channel)
		/// </summary>
		Stereo,
	}
}
