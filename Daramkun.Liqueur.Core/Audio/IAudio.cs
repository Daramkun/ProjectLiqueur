using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Graphics;

namespace Daramkun.Liqueur.Audio
{
	public interface IAudio : IDisposable
	{
		TimeSpan Position { get; set; }
		TimeSpan Duration { get; }

		bool IsPlaying { get; }
		bool IsPaused { get; }

		float Volume { get; set; }
		float Balance { get; set; }
	}
}
