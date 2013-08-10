using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;

namespace Daramkun.Liqueur.Audio
{
	public interface IAudioDevice : IDisposable
	{
		void Play ( IAudio audio );
		void Pause ( IAudio audio );
		void Stop ( IAudio audio );

		void Update ();

		IAudio CreateAudio ( AudioInfo audioInfo );
	}
}
