using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;

namespace Daramkun.Liqueur.Audio
{
	/// <summary>
	/// Audio Device interface
	/// - Audio resource Factory/Manager API
	/// </summary>
	public interface IAudioDevice : IDisposable
	{
		/// <summary>
		/// Play an Audio
		/// </summary>
		/// <param name="audio">Audio resource</param>
		void Play ( IAudio audio );
		/// <summary>
		/// Stop an Audio
		/// </summary>
		/// <param name="audio">Audio resource</param>
		void Pause ( IAudio audio );
		/// <summary>
		/// Stop an Audio
		/// </summary>
		/// <param name="audio">Audio resource</param>
		void Stop ( IAudio audio );

		/// <summary>
		/// Streaming & playing audios
		/// </summary>
		void Update ();

		/// <summary>
		/// Create an audio resource
		/// </summary>
		/// <param name="audioInfo">Audio information & PCM data</param>
		/// <returns>Audio resource</returns>
		IAudio CreateAudio ( AudioInfo audioInfo );

		object Handle { get; }
	}
}
