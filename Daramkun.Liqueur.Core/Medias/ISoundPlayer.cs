using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Decoders;

namespace Daramkun.Liqueur.Medias
{
	public interface ISoundPlayer : IDisposable
	{
		TimeSpan Position { get; set; }
		TimeSpan Duration { get; }

		float Volume { get; set; }

		bool IsPlaying { get; }
		bool IsPaused { get; }

		void Play ();
		void Stop ();
		void Pause ();

		SoundData SoundData { get; }

		bool Update ();
	}
}
