using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Audio
{
	/// <summary>
	/// Audio interface
	/// - Audio resource API
	/// </summary>
	public interface IAudio : IDisposable
	{
		/// <summary>
		/// Current play position
		/// </summary>
		TimeSpan Position { get; set; }
		/// <summary>
		/// Total audio length
		/// </summary>
		TimeSpan Duration { get; }

		/// <summary>
		/// Is state playing?
		/// </summary>
		bool IsPlaying { get; }
		/// <summary>
		/// Is state paused?
		/// </summary>
		bool IsPaused { get; }

		/// <summary>
		/// Audio volume
		/// (0.0 : Min volume, 1.0 : Max volume)
		/// </summary>
		float Volume { get; set; }
		/// <summary>
		/// Audio balance
		/// (-1.0 ~ 0.0 : Left, 0 : Same, 0.0 ~ 1.0 : Right)
		/// </summary>
		float Balance { get; set; }
	}
}
